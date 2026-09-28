# D1-S2-Nb5: Dependencies — Normalisation

Dependencies required to run `d1_s2_nb5_normalisation.ipynb` and `_helpers/nb4_helper.py`.

## Python version

`3.11` (the workshop standard, 3.11.9). The code itself needs `>= 3.9`.

---

## Covered by `pylabrobot` install

Install from GitHub: the notebook uses the v1 Hamilton STAR and Prep devices and the 3D viewer,
which are not in the PyPI release (0.2.2).

```
python -m pip install "pylabrobot[usb] @ git+https://github.com/PyLabRobot/pylabrobot.git"
```

The `[usb]` extra is for the STAR on real hardware (`pylabrobot.io.usb`); simulation does not need
it. The Prep connects over Ethernet and needs no extra.

Not yet on `main`, used by this notebook: `hamilton_plate_carrier_L5_ac` (the STAR's plate carrier).

Pulls in:

| Package | Why |
|---|---|
| `pylabrobot` | devices, resources, liquid handling, the 3D viewer |
| `typing_extensions` | pylabrobot core dependency |
| `websockets` | pylabrobot core dependency (the 3D viewer) |
| `pyusb`, `libusb-package` | via `[usb]`: the STAR over USB |

---

## Install separately

| Package | Why |
|---|---|
| `pandas` | reads and checks the design file (`load_design_file`) |

```
python -m pip install pandas
```

## Execution environment

Not a code import, but needed to *run* the notebook: **`notebook`** (Jupyter Notebook), which
brings `ipython` and top-level `await`.

```
python -m pip install notebook
```

---

## Python stdlib (no install)

`asyncio`, `datetime`, `logging`, `math`, `time`, `tkinter` (`ask_user_choice`)
