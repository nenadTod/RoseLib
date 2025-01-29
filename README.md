# RoseLib

**The idea behind this project is detailed in this [paper.](http://www.eventiotic.com/eventiotic/files/Papers/URL/20bd5c00-5abe-43ff-86b6-64c2941d67ce.pdf)** 

**From the abstract:**
> During our research, we found that Roslyn’s Syntax Tree API is difficult to use due to the inherent properties of its implementation. In this paper, we present our RoseLib library, which abstracts a large part of this implementation, liberates developers from remembering unnecessary details, and makes the development process much more efficient.

## Architecture Overview

Here, I present two groups of classes: navigators and composers. Navigators allow easy selection of Roslyn syntax tree nodes. Composers facilitate changing of selected nodes. 

Both groups have API built with Fluent Interface style in mind. Simplified Class diagrams for both groups are shown beneath.

### Navigators Class Diagram
![Simplified class diagram showing navigators.](https://github.com/nenadTod/RoseLib/blob/master/09%20traversal%20CD.png)

### Composers Class Diagram

![Simplified class diagram showing composers.](https://github.com/nenadTod/RoseLib/blob/master/10%20composing%20CD.png)
