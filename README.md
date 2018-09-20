# Gtk Artificial Life Simulator

## About

![About](/Screenshots/GtkArtificialLife.png)

Gtk Artificial Life Simulator is a user interface for generating, simulating, and visualizing artificial life colonies. Several colony types are supported:

* Life (birth and survival rules)
* Langton Ant's (programmable rule sets)
* Yin-Yang fire
* Zhabotinksy-like reactions
* Forest fires (Planting rate and Fire probabilities)
* Snowflakes
* Stephen Wolfram's Elementary Cellular Automata (ECA, Rules 0-255)
* 2D Turing Machines (programmable)
* Cyclic automatas
* Freezing effects

To generate a colony, select from the available types, modify some parameters including the main color, and neighborhood configuration. Select an area on the world using a click-and-drag mouse pointer motion. Once the left button is released, it will generate a colony (of the chosen type) at the region you have specified. At any time, you can move a colony around the world by first selecting it then hold down the left mouse button and drag the mouse pointer to a different location. Once you release the left mouse button, the colony is moved to the new location.

To erase or nuke a colony, simply right click on it.

At the moment, colonies do not interact with other colonies.

## Load image

![Load image](/Screenshots/LoadImage.png)

Some artificial life colony types allow you to load an image into their world. See how these colonies interact with or react to these patches, e.g. burning in Forest Fire colonies, Langton Ants leaving trails, etc.

# Artificial Life Galleries

## Life

![Life](/Screenshots/Life.png)

Conway's Game of Life

## Elementary CA

![Elementary CA](/Screenshots/ElementaryCA.png)

Stephen Wolfram's Rule 30

## Forest Fire

![Forest Fire](/Screenshots/ForestFire.png)

## Langton Ant

![Langton Ant](/Screenshots/LangtonAnt.png)

Chris Langton's ants

## Snowflake

![Snowflake](/Screenshots/Snowflake.png)

Snowflake generated on a simulated hex-neighborhood

## 2D Turing Machines

![2D Turing Machines](/Screenshots/TuringMachines.png)

## Yin Yang Fire

![Yin Yang Fire](/Screenshots/YinYangFire.png)

## Cyclic Automata

![Cyclic Automata](/Screenshots/Cyclic.png)

## Zhabotinsky Reaction

![Zhabotinsky Reaction](/Screenshots/Zhabotinsky.png)

# Platform

Gtk Artificial Life software has been tested on Linux, OSX, and Windows platforms
