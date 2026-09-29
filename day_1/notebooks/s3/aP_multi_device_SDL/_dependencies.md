# D1-S3-Nb6: Dependencies - Hit the Colour

Dependencies required to run `d1_s3_nb6_hit_the_colour.ipynb` and `_helpers/sdl_helper.py`.

## Python version

`3.11` (the workshop standard, 3.11.9). The code itself needs `>= 3.9`.

---

## Covered by `pylabrobot` install

Install from GitHub: the notebook uses the v1 Hamilton Prep, the Byonoy A96A and the 3D viewer,
which are not in the PyPI release (0.2.2).

```
python -m pip install "pylabrobot[hid] @ git+https://github.com/PyLabRobot/pylabrobot.git"
```

The Prep connects over Ethernet and needs no extra. The Byonoy connects over USB HID
(`pylabrobot.io.hid`); simulation does not need it.

Not yet on `main`, used by this notebook: `CoreGrippers.discard_resource`; a lid dropped by the
grippers on a plate going on top of it; the illumination unit holding a lid on its top face.

| Package | Why |
|---|---|
| `pylabrobot` | the Prep, the Byonoy, resources, the 3D viewer |
| `typing_extensions` | pylabrobot core dependency |
| `websockets` | pylabrobot core dependency (the 3D viewer) |

---

## Install separately

| Package | Why |
|---|---|
| `numpy` | the model: fit and solve |
| `pandas` | the reads, per batch, and the results file |
| `matplotlib` | the live window and the result figure |

```
python -m pip install numpy pandas matplotlib
```

## Execution environment

Not a code import, but needed to *run* the notebook: **`notebook`** (Jupyter Notebook), which
brings `ipython` and top-level `await`.

```
python -m pip install notebook
```

---

## Python stdlib (no install)

`datetime`, `logging`, `os`, `random`, `tkinter` (the live window)
