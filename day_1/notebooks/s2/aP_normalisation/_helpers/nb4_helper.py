"""Helpers for the normalisation notebook: the design file, labware, and user prompts."""

import tkinter as tk

import pandas as pd

from pylabrobot.resources import ContainerRack, Plate, ResourceHolder, create_ordered_items_2d
from pylabrobot.resources.biorad import biorad_96_wellplate_200uL_Vb

REQUIRED_COLUMNS = [
    "plate_id",
    "well_id",
    "sample_name",
    "sample_type",
    "concentration_ng_uL",
    "available sample to dilute (uL)",
]


def load_design_file(path: str) -> pd.DataFrame:
    """Read the design file and sort it column-major (A1, B1, ... H1, A2, ...) within each plate."""
    design = pd.read_csv(path)

    missing = [column for column in REQUIRED_COLUMNS if column not in design]
    if missing:
        raise ValueError(f"{path} is missing the columns {missing}")

    rows = design["well_id"].str[0]
    columns = pd.to_numeric(design["well_id"].str[1:], errors="coerce")
    bad = design.loc[~rows.isin(list("ABCDEFGH")) | ~columns.between(1, 12), "well_id"]
    if not bad.empty:
        raise ValueError(f"not a 96-well position: {bad.tolist()}")

    twice = design.loc[design.duplicated(["plate_id", "well_id"]), ["plate_id", "well_id"]]
    if not twice.empty:
        raise ValueError(f"a well is given twice: {twice.values.tolist()}")

    return (
        design.assign(_row=rows, _col=columns)
        .sort_values(["plate_id", "_col", "_row"])
        .drop(columns=["_row", "_col"])
        .reset_index(drop=True)
    )




def compute_dilutions(
    design_file_df: pd.DataFrame,
    target_concentration_ng_uL: float,
    sample_volume_uL: float,
    max_well_volume_uL: float,
) -> pd.DataFrame:
    """The same sample volume for every sample, topped up with diluent to the target concentration.

    The sample volume is lowered only when it must be: when less is available, or when the diluted
    sample would not fit in the well. Statuses: normalise; sample_adapted; too_dilute (already below
    the target, gets no diluent).
    """
    df = design_file_df.copy()
    concentration = df["concentration_ng_uL"]
    available = df["available sample to dilute (uL)"]
    fits_the_well = max_well_volume_uL * target_concentration_ng_uL / concentration

    sample_uL = available.clip(upper=sample_volume_uL).clip(upper=fits_the_well)
    diluent_uL = (sample_uL * (concentration / target_concentration_ng_uL - 1)).clip(lower=0)

    df["sample_uL"] = sample_uL.round(2)
    df["diluent_uL"] = diluent_uL.round(2)
    df["final_uL"] = (sample_uL + diluent_uL).round(2)
    df["status"] = "normalise"
    df.loc[sample_uL < sample_volume_uL, "status"] = "sample_adapted"
    df.loc[concentration < target_concentration_ng_uL, "status"] = "too_dilute"
    return df


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
            dz=0.0,  # TODO: measure where the trough's base sits in the rack
            item_dx=trough_x + gap_x,
            item_dy=trough_y,
            size_x=trough_x,
            size_y=trough_y,
            size_z=0.0,
            name_prefix=name,
        ),
    )


def new_plates(device: str, design_file_df: pd.DataFrame) -> list[Plate]:
    """One sample plate per plate_id, each sample's well filled as the design file says."""
    plates = []
    for plate_id in design_file_df["plate_id"].unique():
        plate = biorad_96_wellplate_200uL_Vb(name=f"{device}_{plate_id}_input")
        samples = design_file_df[design_file_df["plate_id"] == plate_id]
        for well_id, volume in zip(samples["well_id"], samples["available sample to dilute (uL)"]):
            plate[well_id][0].tracker.set_volume(volume)
        plates.append(plate)
    return plates


def ask_user_choice(title: str, message: str, options: dict[str, object]) -> object:
    """Show a tkinter dialog with buttons for each option, return the selected value.

    The window close (X) button is intentionally disabled so safety-check prompts
    cannot be dismissed without an explicit button click.
    """
    result = [None]

    root = tk.Tk()
    root.withdraw()
    root.attributes("-topmost", True)

    dialog = tk.Toplevel(root)
    dialog.title(title)
    dialog.attributes("-topmost", True)
    dialog.resizable(False, False)

    tk.Label(dialog, text=message, wraplength=400, justify="left", padx=20, pady=15).pack()

    button_frame = tk.Frame(dialog, pady=10)
    button_frame.pack()

    def on_click(value):
        result[0] = value
        dialog.destroy()
        root.destroy()

    for key, value in options.items():
        tk.Button(
            button_frame,
            text=key,
            width=15,
            command=lambda v=value: on_click(v),
        ).pack(side="left", padx=5)

    dialog.protocol("WM_DELETE_WINDOW", lambda: None)
    root.mainloop()

    return result[0]
