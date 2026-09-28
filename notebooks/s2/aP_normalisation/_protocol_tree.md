# D1-S2-Nb5: Protocol Tree — Normalisation

Bring every sample to one concentration: water first (its own volume per sample), then the same
sample volume from every well on top.
Every sample ends at the same concentration in its own volume; an optional last step evens the volumes.
Built on the **Hamilton Prep** first; the STAR follows with the same names and steps.

## Inputs

| Item | Type | Per sample | Source |
|---|---|---|---|
| Samples | 96-well plate, Bio-Rad HSP9601 (`{device}_{plate_id}_input`) | as the design file says | upstream |
| Design file | CSV, one row per sample | `plate_id`, `well_id`, `sample_name`, `sample_type`, `concentration_ng_uL`, `available sample to dilute (uL)` | operator |
| Diluent (water) | 60 mL trough, trough rack position 0 (`{device}_diluent`) | `diluent_uL` | operator |
| 300 uL tips | NTR rack (`{device}_tips_300uL_00`) | — | diluent transfers above 50 uL |
| Normalisation plate (empty) | 96-well plate, Bio-Rad HSP9601 (`{device}_{plate_id}_output`) | — | 1 per input plate |
| 50 uL tips | NTR rack (`{device}_tips_50uL_00`) | — | sample transfers; diluent up to 50 uL |

Set in the notebook (master inputs):

| Parameter | Meaning | Example |
|---|---|---|
| `target_concentration_ng_uL` | the concentration every sample ends at | 10 |
| `sample_volume_uL` | the same sample volume from every well; lowered only when it must be | 10 |
| `max_fill` | fill an output well to at most this share of what it holds | 0.9 |

## Calculations (per sample)

```
sample_uL  = sample_volume_uL                         (the same for every sample)
diluent_uL = sample_uL × (concentration_ng_uL / target_concentration_ng_uL − 1)
final_uL   = sample_uL + diluent_uL                   (varies per sample)
```

`sample_uL` is lowered only when it must be: to what is available, or so that `final_uL` fits the
well (`max_fill` × the output well's volume).

| Status | When | What happens |
|---|---|---|
| `normalise` | the rest | `sample_volume_uL` diluted to the target |
| `sample_adapted` | less is available, or the diluted sample would not fit the well | a smaller sample, diluted to the target |
| `too_dilute` | `concentration_ng_uL < target_concentration_ng_uL` | no diluent, flagged |

---

## Protocol Tree

```
PROTOCOL: Normalisation
│
├── PROCEDURE 0: SET UP & VERIFY                                          (Measure)
│   │
│   ├── Step 0.1: Resolve the design file
│   │   └── load_design_file(): required columns, 96-well positions, no well twice,
│   │       sorted column-major within each plate
│   │
│   ├── Step 0.2: Compute each sample's volumes and status (table above)
│   │
│   ├── Step 0.3: Verify the diluent
│   │   └── Action 0.3a. PROBE: liquid volume in {device}_diluent
│   │       (1 channel, 50 uL tip, cLLD); stop if < sum(diluent_uL) + dead volume
│   │
│   └── Step 0.4: Verify the samples
│       └── Action 0.4a. PROBE: liquid volume in every sample well
│           (2 channels, 50 uL tips, cLLD); flag a sample holding less than sample_uL
│
├── PROCEDURE 1: ADD DILUENT (slide: step 1)                              (Control)
│   │   the tip follows the volume; too_dilute samples get none
│   │
│   ├── Step 1.1: diluent_uL <= 50 uL: 2 x 50 uL tips (blow-out class), reused for the pass
│   ├── Step 1.2: diluent_uL > 50 uL: 2 x 300 uL tips (jet), reused for the pass
│   └── each pass, for each pair of output wells (plate by plate)
│       ├── Action a. ASPIRATE: diluent_uL per channel, each its own volume, from {device}_diluent
│       └── Action b. DISPENSE: into the empty output wells; then discard the tips
│
├── PROCEDURE 2: ADD SAMPLE (slide: step 2)                               (Control)
│   │
│   └── Step 2.1: For each pair of samples, fresh tips
│       ├── Action 2.1a. PICK UP: 2 x 50 uL tips (channels 0 and 1)
│       ├── Action 2.1b. ASPIRATE: sample_uL per channel from the input wells
│       │   (surface, from the probed height)
│       ├── Action 2.1c. DISPENSE: into the diluent in the output wells (surface)
│       ├── Action 2.1d. MIX: 3 cycles, half of final_uL (at most 40 uL: a 50 uL tip)
│       └── Action 2.1e. DISCARD: the tips
│
│   → equimolar, variable-volume samples
│
├── PROCEDURE 3 (optional): EQUAL VOLUMES (slide: step 3)
│   └── Step 3.1: Transfer min(final_uL) from every normalised well to a new plate
│       → equimolar, equal-volume samples
│
└── PROCEDURE 4: RECORD
    └── Step 4.1: Write _results/<run>_normalisation.csv: the design file plus
        probed volume, sample_uL, diluent_uL, final_uL and status, per sample
```

## Why the Prep's two channels, not its 8-channel head

Every sample needs its own volume. The 8-channel head moves one piston for all eight tips, so it
can only give every tip the same volume; the two independent channels each take their own.

## Workshop principles

| Pillar | Here |
|---|---|
| **Measure** | the diluent trough is filled and holds enough (0.3); every sample has what it needs (0.4) |
| **Control** | a volume per channel (1.2, 2.1); a liquid class per channel, by volume |
| **Harness** | a plain CSV as input; AI-generated design files, each run in simulation, to find the edge cases |
| **Own** | the same names and steps on the Prep and the STAR |
