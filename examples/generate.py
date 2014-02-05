#-*- coding: utf-8 -*-

from __future__ import unicode_literals

import io
import subprocess

SYMBOLS = [
    ("a", "#71bf4c"),
    ("53", "#67b8cd"),
    ("X", "#e67e22"),
    ("Tm", "#EE1A45")
]

TEXT_FITTING = [
  ("aa", "AntiAlias"),
  ("aagf", "AntiAliasGridFit"),
  ("ct", "ClearTypeGridFit"),
  ("sbpp", "SingleBitPerPixel"),
  ("sbppgf", "SingleBitPerPixelGridFit")
]

SIZES = [
    (16, 1, 1),
    (16, 2, 2),
    (32, 4, 4),
    (64, 6, 6),
    (120, 12, 12),
]

if __name__ == "__main__":

    index = open("index.rst", "w")

    index.write(
        "Favicon rendering with different text rendering settings\n"
        "========================================================\n\n"
        ".\n\n"
        
        "Glyph rendeding options\n"
        "~~~~~~~~~~~~~~~~~~~~~~~\n\n"
    )

    index.write(
        "{}\n\n".format("\n".join([("- " + x[1]) for x in TEXT_FITTING]))
    )

    for symbol, background in SYMBOLS:
        
        index.write(symbol+"\n")
        index.write("-" * len(symbol) +"\n\n")
        
        for size, padding_v, padding_h in SIZES:
            
            title = "size {} pixels, padding {}, {}".format(size, padding_v, padding_h)
            index.write(title+"\n")
            index.write("+"*len(title) + "\n\n")

            for code, rendering in TEXT_FITTING:
                name = "icon-{}-{}-{}-{}-{}.png".format(symbol, size, code, padding_v, padding_h)

                subprocess.call([
                    "Subpixel.exe",
                    "-t", symbol, "-b", background, "-o", name.encode("utf-8"),
                    "--padding-v", str(padding_v), "--padding-h", str(padding_h),
                    "-w", str(size), "-h", str(size),
                    "-r", rendering, "-s", "Bold"
                ])
                
                index.write(".. image:: {}\n".format(name))
                index.write("    :alt: Rendering: {}\n".format(rendering))

            index.write("\n\n")

    index.close()

    #subprocess.call(["optipng.exe", "-o5", "*.png"], shell=True)
    #subprocess.call(["python", "c:/Python27/Scripts/rst2html.py", "--embed-stylesheet", "index.rst", "index.html"], shell=True)
