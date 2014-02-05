subpixel
========

Favicons with subpixel text rendering::

    subpixel --text S -b #71bf4c --padding-v 1 --padding-h 1 -r AntiAliasGridFit -s Bold -o icon-16.png
    subpixel --text S -b #71bf4c --padding-v 3 --padding-h 3 -r AntiAliasGridFit -s Bold -w 32 -h 32 -o icon-32.png

    ImageMagick/convert.exe icon-16.png icon-32.png favicon.ico
