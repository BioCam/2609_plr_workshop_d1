# PyLabRobot Workshop · Day 1

**From zero to multi-device assays** · Copenhagen · 28 September 2026

Trainers: Rick Wierenga, Camillo Moschner

This repository holds the notebooks for Day 1 of the PyLabRobot workshop. Every notebook runs in
**simulation** first, so you can follow along on your laptop without a robot.

## Agenda

| Time | Session |
|---|---|
| 09:00 – 10:00 | **Session 1 · Intro to PyLabRobot**: the Resource Management System ("digital twin") and the Device Management System ("drivers") |
| 10:00 – 10:15 | Coffee |
| 10:15 – 12:00 | **Session 2 · Probing & verification**, then **Case study I: normalisation**, from simple liquid handling to a managed application |
| 12:00 – 13:00 | Lunch |
| 13:00 – 14:30 | **Session 3 · Case study II: bead-based ELISA**: liquid handler, magnet and plate reader, end to end |
| 14:30 – 14:45 | Coffee |
| 14:45 – 16:00 | **Session 4 · End-user delivery options**, plans & future |

## Notebooks

| Notebook | Session | What you do |
|---|---|---|
| [`d1_s1_nb1_step_by_step`](notebooks/s1_and_s2/d1_s1_nb1_step_by_step.ipynb) | 1 | Build a workcell step by step: a facility, a STARlet and a Prep, and what `setup()` reads from each device, saved as a configuration file |
| [`d1_s1_nb2_hamilton_heaven`](notebooks/s1_and_s2/d1_s1_nb2_hamilton_heaven.ipynb) | 1 | A showcase of the resource model: a whole facility of devices, in 3D |
| [`d1_s2_nb3_probing`](notebooks/s1_and_s2/d1_s2_nb3_probing.ipynb) | 2 | Let the device measure: find the deck, check tip presence, correct the model |
| [`d1_s2_nb4_simple_lh`](notebooks/s1_and_s2/d1_s2_nb4_simple_lh.ipynb) | 2 | Simple liquid handling: one step of a serial dilution, the same code on the Prep, the STAR and the Flex |

## Setup

Please complete this before the workshop. By the end you will have:

- Python 3.11 installed
- a virtual environment (`.venv`) just for the workshop
- PyLabRobot, pandas, matplotlib, tqdm and Jupyter Notebook installed in it
- this repository on your computer, open in Jupyter

We use **Python 3.11.9**: the last Python 3.11 release with official Windows and macOS installers,
so everyone starts from the same setup. If you already have Python 3.11, confirm the version
(step 2) and skip the installer.

Choose **Windows** or **macOS** for steps 1–3; from step 4 on, the commands are the same.

### 1. Install Python 3.11

Go to the official release page: https://www.python.org/downloads/release/python-3119/ and,
under *Files*, download:

| | Installer | Notes |
|---|---|---|
| **Windows** | Windows installer (64-bit) | Windows on ARM: the ARM64 installer. On the first screen, tick **Add python.exe to PATH**, then **Install Now**. |
| **macOS** | macOS 64-bit universal2 installer | Works on Apple Silicon and Intel. Open the `.pkg` and follow the prompts. |

### 2. Confirm Python 3.11

Close any open Command Prompt / Terminal windows and open a new one.

| Windows (Command Prompt) | macOS (Terminal) |
|---|---|
| `py -3.11 --version` | `python3.11 --version` |
| `py -3.11 -m pip --version` | `python3.11 -m pip --version` |

The version should begin with `Python 3.11`.

### 3. Get the workshop and create its environment

**Windows (Command Prompt):**

```bat
git clone https://github.com/BioCam/2609_plr_workshop_d1.git
cd 2609_plr_workshop_d1
py -3.11 -m venv .venv
.venv\Scripts\activate
```

**macOS (Terminal):**

```bash
git clone https://github.com/BioCam/2609_plr_workshop_d1.git
cd 2609_plr_workshop_d1
python3.11 -m venv .venv
source .venv/bin/activate
```

No `git`? Download the repository as a ZIP from GitHub (**Code › Download ZIP**), unzip it, and
`cd` into the folder instead.

Once the environment is active, your prompt begins with `(.venv)`.

### 4. Install the workshop packages

With `(.venv)` active:

```bash
python -m pip install --upgrade pip
python -m pip install "pylabrobot[usb,opentrons] @ git+https://github.com/PyLabRobot/pylabrobot.git"
python -m pip install pandas matplotlib tqdm notebook
```

The notebooks use PyLabRobot's newest devices (the STAR and Prep drivers, the Flex and the 3D
viewer), which are not yet in the release on PyPI, so PyLabRobot is installed from GitHub.

### 5. Check the installation

Run these one at a time. No output from an import means it worked.

```bash
python --version
python -c "import pylabrobot"
python -c "import pandas"
python -c "import matplotlib"
python -c "import notebook"
```

`python --version` should report `Python 3.11.x`, and every import should finish without an error.

### 6. Start Jupyter

From the repository folder, with `(.venv)` still active:

```bash
jupyter notebook
```

Jupyter opens in your browser. Open a notebook from `notebooks/` and run it top to bottom. Keep
the terminal that started Jupyter open while you work; press `Ctrl+C` there to stop it.

### Coming back later

Activate the same environment before running workshop code:

| Windows (Command Prompt) | macOS (Terminal) |
|---|---|
| `cd 2609_plr_workshop_d1` | `cd 2609_plr_workshop_d1` |
| `.venv\Scripts\activate` | `source .venv/bin/activate` |

Leave the environment with `deactivate`.

To update PyLabRobot inside the environment, run the GitHub install from step 4 again with
`--upgrade`. `python -m pip show pylabrobot` shows the installed version.

### Troubleshooting

- **Windows: `py` is not found.** Open a new Command Prompt and retry. If it still fails, rerun
  the Python installer with **Add python.exe to PATH** ticked.
- **macOS: `python3.11` is not found.** Open a new Terminal and retry. If it still fails, rerun
  the Python 3.11 installer.
- **The version is not Python 3.11.** Create the environment with the version-specific command:
  `py -3.11 -m venv .venv` (Windows) or `python3.11 -m venv .venv` (macOS). Once it is active,
  `python --version` reports 3.11.
- **pip installs into the wrong Python.** Don't run plain `pip`: with the environment active, use
  `python -m pip install ...`.
- **macOS: SSL or certificate error.** Open *Applications › Python 3.11*, run
  `Install Certificates.command`, reopen Terminal, reactivate the environment and retry.
- **Windows: PowerShell blocks activation.** Use Command Prompt and `.venv\Scripts\activate`.

## Simulation first

Every notebook starts with a switch per device:

```python
prep_simulation = True  # True or False
star_simulation = True  # True or False
```

With `True`, the device is simulated: it answers from a recording of a real device, so every
command runs and the 3D viewer shows what would happen. Only set a switch to `False` on the
workshop's own robots, with a trainer next to you.

## Devices

- **Hamilton STAR(let)**: simulated throughout
- **Hamilton Prep**
- **Opentrons Flex**: no simulator; its deck is built either way, and the robot is only reached
  in execution

## Learn more

- PyLabRobot on GitHub: https://github.com/PyLabRobot/pylabrobot
- Documentation: https://docs.pylabrobot.org
- Installation guide: https://docs.pylabrobot.org/stable/user_guide/_getting-started/installation.html
- Forum: https://discuss.pylabrobot.org
- Python 3.11.9: https://www.python.org/downloads/release/python-3119/
- pandas: https://pandas.pydata.org/ · matplotlib: https://matplotlib.org/ · Jupyter: https://jupyter.org/

## Thanks

With thanks to our sponsors: Hamilton Company, Opentrons, Byonoy and Alpaqua.
