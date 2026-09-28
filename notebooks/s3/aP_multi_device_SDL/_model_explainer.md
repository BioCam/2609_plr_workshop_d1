# D1-S3-Nb6: The Model - how Hit the Colour finds a recipe

**Its name:** a **classical least squares (CLS) calibration**, also called the **K-matrix method**:
multicomponent analysis under Beer's law. In colour formulation it is Allen's computer colour
matching (1966). Around it, the loop is a model-guided search that zooms in: each batch is a grid
centred on the model's prediction, half as wide as the last.

The question: trough 0 holds an unknown mixture of a yellow and a blue food dye. How much of each
dye, in a 100 uL well, makes the same colour? The Prep answers it by measuring, fitting a small
model, and letting the model choose the next colours to try.

Slide: `day_1/d1_s3_model_slide.pptx`. Code: `_helpers/sdl_helper.py` (`fit_model`,
`solve_recipe`, `grid_batch`) and the notebook's cells D6-D8.

---

## 1. What is measured

Each well is read by the Byonoy at two wavelengths:

| Wavelength | Sees mostly | Because |
|---|---|---|
| 450 nm | the yellow dye (E102) | yellow absorbs blue light |
| 600 nm | the blue dye (E133) | blue absorbs orange-red light |

Every read has the **blank** subtracted: the mean of column 2, 8 wells of water on the same plate.
What is left is the colour the dyes add, not the plate or the water.

So a colour is **two numbers**: `(A450, A600)`. The target is the mean of column 1 minus the blank.

## 2. Beer-Lambert: two straight lines that add up

Two facts about dyes in water (the Beer-Lambert law):

- **Straight line:** twice the dye, twice the absorbance.
- **They add up:** a mix absorbs what each dye absorbs on its own, summed.

For a well with `y` uL of yellow and `b` uL of blue:

```
A450 = y x Y450 + b x B450
A600 = y x Y600 + b x B600
```

The four numbers `Y450, B450, Y600, B600` are **the model**: the absorbance one uL of each dye
adds at each wavelength. Written as a 2 x 2 table:

|  | yellow | blue |
|---|---|---|
| **450 nm** | Y450 (large) | B450 (small) |
| **600 nm** | Y600 (small) | B600 (large) |

## 3. Fitting: learning the model from the wells

Nobody tells the Prep the four numbers: it learns them. Every well it has made is a known recipe
`(y, b)` with a measured colour `(A450, A600)`. `fit_model` finds the four numbers that fit **all
of these wells at once** as closely as possible (least squares: the smallest sum of squared
misses), with every line through zero (no dye, no colour).

- Batch 1 already gives 16 wells, spread over the whole range: enough to fit the model well.
- Every later batch is added to the fit: **more reads, a better model**.

## 4. Solving: from the target's colour to a recipe

With the model known, the target's two readings give two equations with two unknowns, `y` and `b`.
`solve_recipe` solves them (`numpy.linalg.solve`). This is how industry has formulated colours by
computer since 1966 (E. Allen, *J. Opt. Soc. Am.* 56:1256): a matrix inversion of the calibrated
absorbances.

A guess below zero cannot be pipetted, so it is clipped to 0; the notebook also keeps it inside
0-20 uL (`dye_range_uL`).

**A worked example**, with the simulation's model (Y450 = 0.052, B450 = 0.008, Y600 = 0.002,
B600 = 0.071 per uL): a target of 12 uL yellow and 16.4 uL blue reads

```
A450 = 12 x 0.052 + 16.4 x 0.008 = 0.755
A600 = 12 x 0.002 + 16.4 x 0.071 = 1.188
```

Solving those two equations for `y` and `b` gives back 12 and 16.4.

## 5. The loop: aiming each batch

A real model is never exact: the dyes are not perfectly linear, every pipetting step scatters a
little, every read has noise. So the Prep does not trust one answer; it **tries 16 colours around
it and measures**.

| Batch | The 16 recipes | Why |
|---|---|---|
| 1 | a 4 x 4 grid over the whole range, 0-20 uL of each dye | covers everything; these reads calibrate the model |
| 2 | a 4 x 4 grid around the model's guess, 10 uL wide | the guess itself is one of the 16 |
| 3 | the same, 5 uL wide | the search closes in |
| 4, 5 ... | half as wide each time | |

After every batch: **fit** on every read so far, **solve** for the target (the new guess), make the
next grid around it.

**Stop:** when a well's colour is within `tolerance` of the target: the distance between the two
`(A450, A600)` points is below 0.01 absorbance. If a plate fills up first (5 batches), the next plate
continues with its own target and blank; the model keeps every read.

## 6. How close can it get?

Pipetting sets the limit. At a 1% scatter (CV) in volume, 16 uL of blue varies by about 0.16 uL,
which alone moves A600 by about 0.01. So a tolerance of 0.01 is about as tight as the pipetting
allows: a well usually meets it when the model's guess is right and the pipetting lands close.

In simulation (1% pipetting CV, 0.004 reading noise, 100 random targets), all 100 matched: 42 in 2
batches, 3 on average, 6 at most; the recipe found was a median 0.08 uL from the hidden one.

## 7. Where each piece lives

| Idea | Code | Notebook cell |
|---|---|---|
| read, blank-subtracted | `read_batch` | D6 |
| distance to the target | `record` | D7 |
| fit the model | `fit_model` (`sdl_helper.py`) | D8 |
| solve for the recipe | `solve_recipe` (`sdl_helper.py`) | D8 |
| the next 16 recipes | `grid_batch` (`sdl_helper.py`) | D1, D8 |
| the simulated reader: Beer-Lambert plus scatter | `SimulatedReader` (`sdl_helper.py`) | A |

## 8. What the simulation assumes

- The dyes follow Beer-Lambert exactly, with the four numbers above. Real dyes are close to linear
  below about 2.0 absorbance; the calibration wells show how close.
- Pipetting scatters 1% around the asked volume; each read has 0.004 absorbance of noise.
- The wells are perfectly mixed before every read.
