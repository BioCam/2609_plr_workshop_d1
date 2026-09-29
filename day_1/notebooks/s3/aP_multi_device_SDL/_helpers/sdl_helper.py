"""Helpers for the hit-the-colour notebook: the trough rack, the model, a stand-in reader, the plot."""

import random

import numpy as np

from pylabrobot.resources import ContainerRack, ResourceHolder, create_ordered_items_2d
from pylabrobot.resources.hamilton import hamilton_1_trough_60mL_Vb

DYES = ("yellow", "blue")


def hamilton_5_troughrack_60mL(name: str) -> ContainerRack:
    """5x 60 mL troughs, evenly spaced and centred on an SBS footprint."""
    n_troughs, trough_x, trough_y = 5, 19.0, 90.0
    gap_x = (127.76 - n_troughs * trough_x) / (n_troughs + 1)
    return ContainerRack(
        name=name,
        size_x=127.76,
        size_y=85.48,
        size_z=40.0,
        ordered_items=create_ordered_items_2d(
            klass=ResourceHolder,
            num_items_x=n_troughs,
            num_items_y=1,
            dx=6.96,
            dy=-5.52,  # a 90 mm trough overhangs the 85.48 mm footprint to the front
            dz=4.77,  # z-touch: the rack floor stands at 8.27 mm on the Prep deck
            item_dx=trough_x + gap_x,
            item_dy=trough_y,
            size_x=trough_x,
            size_y=trough_y,
            size_z=0.0,
            name_prefix=name,
        ),
    )


def hamilton_1_trough_60mL_Vb_measured(name: str):
    """A 60 mL V-bottom trough, its cavity bottom where the needle meets it: 5.45 mm above its base."""
    trough = hamilton_1_trough_60mL_Vb(name=name)
    trough._material_z_thickness = 5.45  # z-touch: 13.72 mm, on a base at 8.27 mm
    return trough


def solve_recipe(model, target):
    """uL of each dye that give the target's readings: Allen's matrix method (1966).

    Args:
      model: 2x2 absorbance per uL; rows are wavelengths, columns are dyes.
      target: the target's readings at the same wavelengths, blank subtracted.
    """
    return np.clip(np.linalg.solve(model, target), 0.0, None)


class SimulatedReader:
    """Reads what was pipetted, by Beer-Lambert, with pipetting and reading scatter.

    Stands in for the Byonoy in simulation. Every transfer is recorded as it is pipetted.

    Args:
      troughs: what each trough holds, by name: uL of each dye stock per uL of liquid.
      pipetting_cv: how much a delivered volume scatters around the asked one.
      reading_sd: the noise on every read, in absorbance.
      seed: makes the scatter repeatable.
    """

    # absorbance per uL of dye stock in a well: read straight down, the amount of dye counts
    ABSORBANCE_PER_UL = {"yellow": {450: 0.052, 600: 0.002}, "blue": {450: 0.008, 600: 0.071}}
    BLANK = {450: 0.045, 600: 0.041}

    def __init__(self, troughs, pipetting_cv: float = 0.01, reading_sd: float = 0.004, seed: int = 1):
        self.troughs = troughs
        self.contents = {}  # well name -> {dye: uL of dye stock}
        self.pipetting_cv = pipetting_cv
        self.reading_sd = reading_sd
        self.random = random.Random(seed)

    def new_plate(self) -> None:
        """Forget what was pipetted: the next plate's wells start empty."""
        self.contents = {}

    def add(self, well_name: str, trough_name: str, uL: float) -> None:
        """Record a transfer, as the robot delivers it: close to the asked volume, not exact."""
        delivered = uL * (1 + self.random.gauss(0, self.pipetting_cv))
        well = self.contents.setdefault(well_name, {d: 0.0 for d in DYES})
        for dye, per_uL in self.troughs[trough_name].items():
            well[dye] += delivered * per_uL

    def read(self, well_names, wavelength: int):
        """Each well's absorbance at the wavelength."""
        readings = {}
        for name in well_names:
            dyes = self.contents.get(name, {d: 0.0 for d in DYES})
            a = self.BLANK[wavelength] + sum(
                uL * self.ABSORBANCE_PER_UL[d][wavelength] for d, uL in dyes.items()
            )
            readings[name] = a + self.random.gauss(0, self.reading_sd)
        return readings


def fit_model(recipes, reads):
    """Absorbance per uL of each dye at each wavelength, by least squares over every read so far.

    Args:
      recipes: n x 2, uL of yellow and blue per well.
      reads: n x w, each well's blank-subtracted absorbance at each wavelength.

    Returns:
      w x 2: rows are wavelengths, columns are dyes (Beer-Lambert, a line through zero).
    """
    model, *_ = np.linalg.lstsq(np.asarray(recipes, float), np.asarray(reads, float), rcond=None)
    return model.T


def grid_batch(centre, width, low, high, n_per_side=4):
    """n_per_side x n_per_side recipes on a square grid around a centre, kept inside [low, high].

    The grid point nearest the centre is the centre itself, so every batch tries the best guess.
    """
    half = width / 2
    axes = [
        np.linspace(max(low, c - half), min(high, c + half), n_per_side) for c in centre
    ]
    grid = [(float(y), float(b)) for y in axes[0] for b in axes[1]]
    nearest = min(range(len(grid)), key=lambda i: np.hypot(*np.subtract(grid[i], centre)))
    grid[nearest] = (float(centre[0]), float(centre[1]))
    return grid


def colour_of(a450, a600):
    """Roughly what a well looks like: 450 nm takes out blue light, 600 nm takes out red."""
    clip = lambda v: float(min(1.0, max(0.0, v)))  # noqa: E731
    return (clip(10 ** -a600), clip(10 ** -(0.12 * (a450 + a600))), clip(10 ** -a450))


def draw_progress(fig, target, tolerance, batches):
    """Draw the loop so far: the colours per batch, the search in recipe space, the distance.

    Args:
      fig: a matplotlib Figure, cleared and redrawn.
      target: the target's (A450, A600), blank subtracted.
      tolerance: the distance that counts as a match.
      batches: one dict per batch so far: "recipes" n x 2 (uL yellow, blue), "reads" n x 2,
        "distances" n.
    """
    from matplotlib.patches import Rectangle

    fig.clear()
    swatches, space, closing = fig.subplots(1, 3, gridspec_kw={"width_ratios": [4, 3, 2]})

    # colours: one row per batch, the target on the left, the best well boxed
    n = max(len(b["reads"]) for b in batches)
    rows = max(len(batches), 5)
    swatches.add_patch(Rectangle((-1.6, -rows + 0.1), 1.2, rows - 0.2, color=colour_of(*target)))
    swatches.text(-1.0, 0.3, "target", ha="center", fontsize=9, color="#12384A")
    for i, b in enumerate(batches):
        best = int(np.argmin(b["distances"]))
        for j, r in enumerate(b["reads"]):
            swatches.add_patch(Rectangle((j * 0.5, -i - 0.9), 0.45, 0.8, color=colour_of(*r)))
        swatches.add_patch(Rectangle((best * 0.5 - 0.03, -i - 0.93), 0.51, 0.86, fill=False,
                                     ec="#FEBE04", lw=2.5))
        swatches.text(-0.2, -i - 0.5, str(i + 1), ha="right", va="center", fontsize=10,
                      color="#12384A")
    swatches.set_xlim(-1.8, n * 0.5)
    swatches.set_ylim(-rows - 0.1, 0.7)
    swatches.axis("off")
    swatches.set_title("16 colours per batch, best boxed", color="#12384A")

    # recipe space: each batch's grid, closing in
    for i, b in enumerate(batches):
        rec = np.asarray(b["recipes"])
        space.scatter(rec[:, 0], rec[:, 1], s=40 + 25 * i, c=[colour_of(*r) for r in b["reads"]],
                      edgecolors="#12384A", linewidths=0.8, zorder=2 + i)
        best = rec[int(np.argmin(b["distances"]))]
        space.scatter([best[0]], [best[1]], s=200, facecolors="none", edgecolors="#FEBE04",
                      linewidths=2, zorder=10)
    space.set_xlabel("uL yellow")
    space.set_ylabel("uL blue")
    space.set_title("the search closes in", color="#12384A")
    space.grid(color="#C9E5EE", lw=0.8)

    # distance per batch
    best_distances = [float(np.min(b["distances"])) for b in batches]
    closing.bar([str(i + 1) for i in range(len(batches))], best_distances, color="#12384A")
    closing.axhline(tolerance, color="#FEBE04", lw=2, label=f"tolerance {tolerance}")
    closing.set_yscale("log")
    closing.set_xlabel("batch")
    closing.set_ylabel("best well's distance to the target")
    closing.set_title("closing in", color="#12384A")
    closing.legend(fontsize=8, frameon=False)
    fig.tight_layout()


def open_progress_window(target, tolerance, title="Hit the Colour"):
    """A tkinter window that redraws the loop after every batch.

    Returns:
      show(batches): redraw with the batches so far; close(): close the window.
    """
    import tkinter as tk

    from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
    from matplotlib.figure import Figure

    root = tk.Tk()
    root.title(title)
    fig = Figure(figsize=(13, 5), dpi=100)
    canvas = FigureCanvasTkAgg(fig, master=root)
    canvas.get_tk_widget().pack(fill="both", expand=True)

    def show(batches):
        draw_progress(fig, target, tolerance, batches)
        canvas.draw()
        root.update()

    def close():
        root.destroy()

    root.update()
    return show, close
