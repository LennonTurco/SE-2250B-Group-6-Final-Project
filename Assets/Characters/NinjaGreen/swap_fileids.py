"""
Swap Boy fileIDs to Flam fileIDs in all .anim files under Flam/Animations.

Parses both SpriteSheet.png.meta files to build a mapping:
  Boy fileID -> sprite name -> Flam fileID
Then replaces every occurrence in the .anim files.
"""

import re
import os
import glob

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PROJECT_ROOT = os.path.abspath(os.path.join(SCRIPT_DIR, "..", ".."))

BOY_META = os.path.join(PROJECT_ROOT, "Characters", "Boy", "SpriteSheet.png.meta")
NINJA_META = os.path.join(PROJECT_ROOT, "Characters", "Flam", "SpriteSheet.png.meta")
ANIM_DIR = os.path.join(SCRIPT_DIR, "..", "Flam", "Animations")


def parse_name_fileid_table(meta_path: str) -> dict[str, str]:
    """Parse the internalIDToNameTable from a .meta file.
    
    Returns a dict mapping sprite name -> fileID string.
    We parse the YAML-like structure:
      - first:
          213: <fileID>
        second: <spriteName>
    """
    name_to_id: dict[str, str] = {}

    with open(meta_path, "r", encoding="utf-8") as f:
        content = f.read()

    # Parse internalIDToNameTable entries
    # Pattern: "213: <fileID>" followed by "second: <name>"
    pattern = re.compile(
        r"-\s+first:\s*\n\s+213:\s+(-?\d+)\s*\n\s+second:\s+(\S+)",
        re.MULTILINE
    )
    for match in pattern.finditer(content):
        file_id = match.group(1)
        sprite_name = match.group(2)
        name_to_id[sprite_name] = file_id

    # Also parse the spriteSheet.sprites section for internalID values,
    # which may have additional/different IDs not in internalIDToNameTable
    sprite_pattern = re.compile(
        r"name:\s+(\S+)\s*\n(?:.*?\n)*?\s+internalID:\s+(-?\d+)",
        re.MULTILINE
    )
    for match in sprite_pattern.finditer(content):
        sprite_name = match.group(1)
        internal_id = match.group(2)
        # The spriteSheet entries are the ground truth for actual fileIDs
        name_to_id[sprite_name] = internal_id

    return name_to_id


def build_replacement_map(boy_meta: str, ninja_meta: str) -> dict[str, str]:
    """Build a map of Boy fileID -> Flam fileID for sprites with the same name."""
    boy_ids = parse_name_fileid_table(boy_meta)
    ninja_ids = parse_name_fileid_table(ninja_meta)

    replacement: dict[str, str] = {}
    for sprite_name, boy_id in boy_ids.items():
        if sprite_name in ninja_ids:
            ninja_id = ninja_ids[sprite_name]
            if boy_id != ninja_id:
                replacement[boy_id] = ninja_id

    return replacement


def swap_fileids_in_anim(anim_path: str, replacements: dict[str, str]) -> int:
    """Replace Boy fileIDs with Flam fileIDs in a single .anim file.
    
    Returns the number of replacements made.
    """
    with open(anim_path, "r", encoding="utf-8") as f:
        content = f.read()

    count = 0
    new_content = content

    for boy_id, ninja_id in replacements.items():
        # Match fileID references like: {fileID: -4768450242260759221,
        pattern = re.compile(r"(fileID:\s*)" + re.escape(boy_id) + r"(\s*[,}])")
        matches = pattern.findall(new_content)
        if matches:
            count += len(matches)
            new_content = pattern.sub(r"\g<1>" + ninja_id + r"\2", new_content)

    if count > 0:
        with open(anim_path, "w", encoding="utf-8") as f:
            f.write(new_content)

    return count


def main():
    print("=== Flam Animation FileID Swapper ===\n")

    # Verify paths exist
    for path, label in [(BOY_META, "Boy SpriteSheet.meta"), (NINJA_META, "Flam SpriteSheet.meta"), (ANIM_DIR, "Animations dir")]:
        if not os.path.exists(path):
            print(f"ERROR: {label} not found at: {path}")
            return

    # Build replacement map
    replacements = build_replacement_map(BOY_META, NINJA_META)
    print(f"Found {len(replacements)} fileID differences between Boy and Flam:\n")
    for boy_id, ninja_id in sorted(replacements.items(), key=lambda x: x[0]):
        print(f"  Boy {boy_id:>25s}  ->  Flam {ninja_id}")
    print()

    # Process all .anim files
    anim_files = sorted(glob.glob(os.path.join(ANIM_DIR, "*.anim")))
    if not anim_files:
        print("No .anim files found!")
        return

    total = 0
    for anim_path in anim_files:
        basename = os.path.basename(anim_path)
        count = swap_fileids_in_anim(anim_path, replacements)
        if count > 0:
            print(f"  {basename}: {count} fileID(s) replaced")
            total += count
        else:
            print(f"  {basename}: no changes needed")

    print(f"\nDone! {total} total replacement(s) across {len(anim_files)} file(s).")


if __name__ == "__main__":
    main()
