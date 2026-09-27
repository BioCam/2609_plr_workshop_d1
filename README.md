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

## Getting started

You need Python 3.9 or newer and Jupyter.

```bash
git clone https://github.com/BioCam/2609_plr_workshop_d1.git
cd 2609_plr_workshop_d1

python -m venv .venv
source .venv/bin/activate          # Windows: .venv\Scripts\activate

pip install "pylabrobot[usb,opentrons] @ git+https://github.com/PyLabRobot/pylabrobot.git"
pip install jupyterlab tqdm

jupyter lab
```

Open a notebook from `notebooks/` and run it top to bottom.

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
- Forum: https://discuss.pylabrobot.org

## Thanks

With thanks to our sponsors: Hamilton Company, Opentrons, Byonoy and Alpaqua.
