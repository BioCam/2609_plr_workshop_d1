# D1-S2-Nb5: Normalisation

Bring every sample to one concentration. A design file lists each sample's concentration; the
protocol takes the same volume of every sample, works out how much water brings each one to the
target, puts the water into an empty plate, and the sample on top. Every sample ends at the same concentration in its
own volume; an optional last step evens the volumes.

Case study I of the PyLabRobot workshop, Day 1, Session 2. Built on the **Hamilton Prep** first; the
STAR follows with the same names and steps.

- **Throughput:** 1–192 samples (1–2 input plates per device)
- **Plate layout:** adaptive. Only the wells in the design file are touched.

## Inputs / Outputs

| Input | Output |
|---|---|
| 1–2 × 96-well plate of samples | 1–2 × 96-well plate, every sample at `target_concentration_ng_uL`, each in its own volume |
| Design file (CSV, one row per sample) | `_results/<run>_normalisation.csv`: per sample, what was asked, measured and done |
| Water (diluent), 60 mL trough | |

## Entry points

| File | Purpose |
|---|---|
| `d1_s2_nb5_normalisation.ipynb` | The protocol: run this |
| `_protocol_tree.md` | Step-by-step protocol structure |
| `_design_templates/d1_s2_nb5_design_example.csv` | An example design file: 24 samples on one plate |

## Design file

One row per sample:

| Column | Meaning |
|---|---|
| `plate_id` | which input plate, e.g. `plate_1`; one plate per id, at most 2 |
| `well_id` | its well, `A1` … `H12` |
| `sample_name` | free text |
| `sample_type` | e.g. `DNA`, `RNA` |
| `concentration_ng_uL` | the measured concentration |
| `available sample to dilute (uL)` | how much of it there is |

The notebook checks the columns, that every well is a real 96-well position and that no well is
given twice, then sorts the samples column-major within each plate.

## Master inputs (in the notebook)

| Parameter | Meaning |
|---|---|
| `prep_simulation`, `star_simulation` | `True` simulates the device, `False` drives it |
| `design_file_directory` | path to the design file |
| `target_concentration_ng_uL` | the concentration every sample ends at |
| `sample_volume_uL` | the same sample volume from every well; lowered only when it must be |
| `max_fill` | fill an output well to at most this share of what it holds (0.9) |

Per sample: `diluent_uL = sample_uL × (concentration / target − 1)`, so `final_uL = sample_uL + diluent_uL`:
the water is what varies. The sample volume is lowered only when less is available or the diluted
sample would not fit the well; samples already below the target get no water (see `_protocol_tree.md`).

## Deck (Prep)

| Site | Holds |
|---|---|
| 0 | trough rack, water in position 0 (`prep_diluent`) |
| 1 | 50 uL NTR tips (`prep_tips_50uL_00`) |
| 2, 3 | normalisation plates (`prep_<plate_id>_output`) |
| 4, 5 | sample plates (`prep_<plate_id>_input`) |
| 7 | 300 uL NTR tips (`prep_tips_300uL_00`) |

## Validation status

**In development.** On the Prep, the design file, volumes, diluent and sample steps and the results
file run in simulation. Not yet run on hardware. Next: the verification probes (steps 0.3, 0.4), the
optional equal-volume step, and the STAR.

## Required resources

| Resource | Type | OEM | Cat. No. | PLR name |
|---|---|---|---|---|
| Prep | Liquid handler | Hamilton | — | `PrepDevice` |
| 96-well PCR plate | Skirted plate | Bio-Rad | HSP9601 | `biorad_96_wellplate_200uL_Vb` |
| 60 mL trough | Trough | Hamilton | 56694-01 | `hamilton_1_trough_60mL_Vb` |
| 50 uL tips | NTR tip rack | Hamilton | — | `hamilton_96_tiprack_50uL_NTR` |
| 300 uL tips | NTR tip rack | Hamilton | — | `hamilton_96_tiprack_300uL_NTR` |
