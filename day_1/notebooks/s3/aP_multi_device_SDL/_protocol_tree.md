# D1-S3-Nb6: Protocol Tree - Hit the Colour

Find the recipe of an unknown colour from two food dyes, closed-loop, in batches of 16.
Prep only, with the Byonoy A96A on the deck. No liquid-level detection: transparent tips.
The step names (B, C1-C7, D1-D8, E) are the notebook's cells.

## Inputs

| Item | Type | Per run | Source |
|---|---|---|---|
| Target mixture | 60 mL trough, rack position 0 (`target`) | 8 x 100 uL per plate | operator |
| Water | 60 mL trough, rack position 1 (`water`) | 8 x 100 uL per plate + top-ups | operator |
| Yellow dye (E102) | 60 mL trough, rack position 2 (`yellow`) | up to 20 uL a well | operator |
| Blue dye (E133) | 60 mL trough, rack position 3 (`blue`) | up to 20 uL a well | operator |
| Plates | Corning 96, flat bottom, lidded, stack at site 7 | 1-2 | operator |
| 50 uL tips | NTR rack, site 1 | 6 per batch | the 2 channels |
| 300 uL tips | NTR racks, sites 6 and 5 | a rack per plate | the 8-channel head |

## The model

```
A450 = y x Y450 + b x B450        Y, B: absorbance per uL of yellow and blue,
A600 = y x Y600 + b x B600        least squares over every read so far (Beer-Lambert)

guess = solve(model, target)      Allen's matrix method (1966)
next batch = 4 x 4 grid around the guess, half as wide, the guess included
```

---

## Protocol Tree

```
PROTOCOL: Hit the Colour
│
├── PART A: SET UP                                                          (Model)
│   └── the Prep, the Byonoy units, the troughs, the tip racks and the plate stack on the deck
│
├── PART B: MEASURE EVERY TROUGH (execution only)                           (Measure)
│   └── the teaching needle, cLLD, 3 probes each: every trough's volume
│       → every later aspirate: computed liquid height, surface following, LLD off
│
├── PART C: THE FIRST PLATE                                                 (Control, Measure)
│   ├── C1. MOVE: the illumination unit onto the parking unit (y_clearance 2)
│   ├── C2. MOVE: the top plate off the stack onto the detection unit
│   ├── C3. MOVE: its lid on top of the illumination unit
│   ├── C4. HEAD: 100 uL of the target into column 1; discard the tips
│   ├── C5. HEAD: 100 uL of water into column 2 (the blank); discard the tips
│   ├── C6. READ: illumination unit on, columns 1 and 2 at 450 and 600 nm, unit off
│   │       → target = mean(column 1) - mean(column 2)
│   └── C7. OPEN the live window
│
├── PART D: BATCH 1, STEP BY STEP (columns 3 and 4)                         (Control, Harness)
│   ├── D1. the 16 recipes: a 4 x 4 grid over 0-20 uL of each dye
│   ├── D2. YELLOW: both channels, two wells at a time, one pair of tips, 3 mm above the liquid
│   ├── D3. BLUE: the same
│   ├── D4. WATER: to 100 uL in every well, the same
│   ├── D5. MIX: each column with the head, a column of tips each
│   ├── D6. READ: illumination unit on, the batch and the blank, unit off
│   ├── D7. HOW CLOSE: each well's distance to the target; the live window updates
│   └── D8. MODEL: fit on every read so far, solve for the target, the next 16 recipes
│
├── PART E: THE LOOP                                                        (all four)
│   ├── when a plate is full (5 batches):
│   │   ├── MOVE: the lid back onto the plate
│   │   ├── DISCARD: the closed plate past the waste block, into the trash
│   │   └── C2-C6 again on the next plate, with its own rack of 300 uL tips
│   └── D1-D8, batch after batch, until a well is within tolerance (0.01)
│
└── PART F: THE RESULT
    ├── the figure: colours per batch, the search closing in, the distance per batch
    ├── _results/<run>_hit_the_colour.csv: every recipe and its reads
    ├── CLOSE the live window
    └── shutdown
```

## Why both channels, and the head

Every well needs its own dye and water volumes: the 2 independent channels each take their own,
two wells at a time. Dispensing above the liquid already there means a tip never touches another
liquid, so each liquid keeps one pair of tips for the whole batch. The target, the blank and the
mixing are the same in every well of a column: the 8-channel head does a column at once.

## Workshop principles

| Pillar | Here |
|---|---|
| **Measure** | the troughs by the teaching needle; the target against a blank; every batch read |
| **Control** | each well its own volumes; computed heights instead of LLD; the head, the channels and the grippers each doing their part |
| **Harness** | numpy fits and solves the model (Allen 1966); the next batch follows from the data |
| **Own** | a new gripper command (`discard_resource`); the Byonoy units and the trough rack modelled on the Prep deck |
