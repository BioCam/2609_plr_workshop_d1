"""Real Flex smoke test: tips in C1, Bio-Rad plate in D2, P50 on the right.

Run from the workshop root:
  PYTHONPATH=pylabrobot ./env/bin/python scripts/flex_mix_smoke.py

Homes during setup. The first plate column must contain at least 50 uL per
well; bookkeeping assumes 100 uL initially. Returns tips only after success.
"""

import asyncio

from pylabrobot.opentrons import Flex, FlexHead8
from pylabrobot.resources import set_tip_tracking, set_volume_tracking
from pylabrobot.resources.biorad import biorad_96_wellplate_200uL_Vb
from pylabrobot.resources.opentrons import FlexDeck, flex_96_tiprack_50ul


async def main() -> None:
    """Pick up one column, mix in place, and return the same tips."""
    deck = FlexDeck()
    tips = flex_96_tiprack_50ul("tips")
    plate = biorad_96_wellplate_200uL_Vb("plate")
    deck.assign_child_at_slot(tips, "C1")
    deck.assign_child_at_slot(plate, "D2")
    set_tip_tracking(True)
    set_volume_tracking(True)
    for well in plate.column(0):
        well.tracker.set_volume(100)

    flex = Flex(serial_port="/dev/tty.usbmodem1201", deck=deck)
    try:
        print("Setting up Flex (homes axes)", flush=True)
        await flex.setup()
        p50 = flex.right
        if not isinstance(p50, FlexHead8) or p50.max_volume != 50:
            raise RuntimeError("Expected the P50 8-channel pipette on the right")

        print("Picking up tips A1:H1 from C1", flush=True)
        await p50.pick_up_tips(tips.column(0))

        print("Mixing D2 A1:H1: 5 x 50 uL, 4 mm above bottom", flush=True)
        await p50.mix(plate.column(0), volume=50, repetitions=5, liquid_height=4)

        print("Returning tips to C1 A1:H1", flush=True)
        await p50.drop_tips(tips, column=0)
        print("PASS: pickup, mix, and tip return completed", flush=True)
        print("Tracked well volumes:", [w.tracker.volume for w in plate.column(0)], flush=True)
    finally:
        await flex.disconnect()


if __name__ == "__main__":
    asyncio.run(main())
