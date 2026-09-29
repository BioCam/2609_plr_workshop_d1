# D1-S3-Nb6: Hit the Colour

Give the Prep a mixture of 2 dyes and it finds the recipe for itself itself.

Trough 0 holds an unknown mixture of a yellow and a blue food dye.
The Prep reads it on the Byonoy, then mixes colours in batches of 16, reads them, fits a model to every read so far, and aims the next batch at the model's best guess, until a well matches the target.

Case study II of the PyLabRobot workshop, Day 1, Session 3. **Prep only**, with the Byonoy A96A on its deck.

- **Method:** computer colour matching, Allen, *J. Opt. Soc. Am.* 56:1256 (1966): the recipe comes from a matrix inversion of the calibrated absorbances.
- **Physics:** Beer-Lambert; the absorbances of two dyes add up.
- **Same physics, real standard:** two dyes at two wavelengths is how ISO 8655-7:2022 (Annex B)
  verifies pipetting volumes.

## Entry points

| File | Purpose |
|---|---|
| `d1_s3_nb6_hit_the_colour.ipynb` | The protocol: run this, one cell at a time with the 3D viewer open |
| `_protocol_tree.md` | Step-by-step protocol structure |
| `_model_explainer.md` | The model in plain terms: what is measured, fit, solve, the loop, its limits |
| `_dependencies.md` | What to install |
| `_helpers/sdl_helper.py` | The trough rack, the model (fit, solve, next grid), the simulated reader, the live plot |

## How the notebook is built

Every physical step has its own cell and runs once by hand. Each cell defines the small function it
runs, so the loop at the end is made of exactly the steps already watched.

| Part | Cells |
|---|---|
| A. Set up | modes, master inputs, logging, the 3D viewer, the Prep, the deck, the Byonoy |
| B. Measure every trough | the teaching needle, 3 probes per trough (execution only) |
| C. The first plate | C1 illumination unit to parking; C2 plate onto the detection unit; C3 lid on top of the illumination unit; C4 target into column 1; C5 blank into column 2; C6 read the target; C7 open the live window |
| D. Batch 1 | D1 the 16 recipes; D2 yellow; D3 blue; D4 water; D5 mix; D6 read; D7 how close; D8 the model and the next recipes |
| E. The loop | the plate change (lid back on, discard, the next plate from C2); then D1-D8 until matched |
| F. The result | the figure, the results file, close the live window, shutdown |

## Master inputs

| Parameter | Meaning |
|---|---|
| `prep_simulation`, `reader_simulation` | `True` simulates the device, `False` drives it |
| `seed` | simulation: draws the hidden target and all scatter; the same seed, the same run |
| `well_uL` | every well: the target, or yellow + blue + water (100) |
| `wavelengths` | 450 nm (yellow), 600 nm (blue) |
| `dye_range_uL` | how much of each dye a recipe may take (0-20) |
| `batches_per_plate` | 16 colours each, in columns 3-12 (5) |
| `max_plates` | a full plate goes to the trash and the next comes off the stack (2) |
| `plates_in_stack` | lidded plates on the stack at the start (4) |
| `tolerance` | stop when a well is this close to the target (0.01) |
| `above_surface_mm` | dispense this far above the liquid, so a tip never touches another liquid (3) |

A simulated reader computes each well's absorbance from what the Prep pipetted into it
(Beer-Lambert, 1% pipetting and 0.004 reading scatter): PyLabRobot has no Byonoy simulator.

## Deck (Prep)

| Site | Holds |
|---|---|
| 0 | Byonoy detection unit: the plate in use sits on it for the whole plate |
| 1 | 50 uL NTR tips: a pair for each liquid, each batch; a rack per run |
| 2 | Byonoy parking unit: the illumination unit waits here while the Prep pipettes |
| 3 | free (at the back, out of the grippers' reach) |
| 4 (`slot_1_0`) | trough rack: target (0), water (1), yellow (2), blue (3) |
| 5 | 300 uL NTR tips for the second plate |
| 6 | 300 uL NTR tips for the first plate: target, blank, one column per batch column to mix |

Every tip pick-up takes the next full column (head) or pair (channels) the rack has:
`find_tip_spots(rack, has_tip=True, count=..., x_aligned=True)`.
| 7 | a stack of lidded plates (Corning 96, flat bottom) |

A plate's lid rests on top of the illumination unit and travels with it; it goes back on the plate
before a full plate is thrown into the trash past the waste block.

## Consumables per run

| Item | Per run |
|---|---|
| Plates | 1, or 2 if the first fills up |
| 50 uL tips | 6 per batch, up to 60: a rack per run |
| 300 uL tips | a rack per plate |

## Validation status

**In development; not yet run on hardware.** The whole notebook runs in simulation, with one plate
and with a plate change (a tolerance too tight to meet sends it to the second plate). Over 100
seeded random targets (seeds 1-100), all 100 matched: 42 in 2 batches, 35 in 3, the mean at 3.1,
the slowest in 6; 92 on one plate, 8 on two. The recipe found was a median 0.08 uL, at most 0.3 uL,
from the hidden one. Pipetting scatter sets how close any match can get: at 1% CV and 16 uL it
alone moves the reading by about 0.01.

**Time, estimated** (from operation counts, not yet measured): setup about 10 min, then about
11 min per batch and 7 min per plate change. A typical run (2-3 batches) takes about 30-45 min,
the slowest in the simulation (6 batches, two plates) about 80.

Before the first real run, rehearse:
- the teaching needle probing all four troughs;
- the gripper moves: the illumination unit to the parking unit and back (`y_clearance=2`); a plate
  off the stack onto the detection unit; its lid onto the illumination unit and back onto the
  plate; `discard_resource` throwing a closed plate past the waste block;
- both channels aspirating from one trough and dispensing 3 mm above the liquid; the 8-channel head
  in a 60 mL trough and on the plate on the detection unit;
- that the dyes (E102, E133) read below about 2.0, and that head mixing colours the wells evenly.

## Required resources

| Resource | Type | OEM | PLR name |
|---|---|---|---|
| Prep | Liquid handler | Hamilton | `Prep` |
| A96A | Absorbance reader | Byonoy | `byonoy_a96a_detection_unit`, `..._parking_unit`, `..._illumination_unit` |
| 96-well plate, flat bottom, lidded | Plate | Corning | `Cor_96_wellplate_360ul_Fb` |
| 60 mL trough | Trough | Hamilton | `hamilton_1_trough_60mL_Vb` |
| 50 uL tips | NTR tip rack | Hamilton | `hamilton_96_tiprack_50uL_NTR` |
| 300 uL tips | NTR tip rack | Hamilton | `hamilton_96_tiprack_300uL_NTR` |
| Teaching needle | on the Prep deck | Hamilton | `Prep_teaching_needle` |
