# XMetrics
## A simple metrics tool window for .Net projects.

## Installation

Download and extract Release/Release.zip and then run the VISX installer within.

To run XMetrics, after installing go to View > Other Windows > XMetrics.  You'll be able to then analyse any built assemblies in your solution.

The analysis returns each class and, for each class, a measure of its cohesion and its coupling.

## Coupling

This metric is indicative of how integrated this class is with your system.  IE "If I were to make changes to this class, how big will the knock-on effects be?".  There is no definitive answer as to how coupled is too-coupled.  It depends on your solution.  Generally the higher the number here, the greater the use that this class is made use of, directly, by the system.

Technically it is the ratio of incoming calls to the class vs. the number of outgoing calls made by the class.  For more information, please see this paper: http://searchdl.org/public/book_series/elsevierst/6/548.pdf

## Cohesion

This is a measure of how much the class components belong to each other.  Or, to put it another way, how much the components of the class relate to a central purpose.  It's a little tickley to measure, but Uncle Bob recommends that one way is to compare the fields within the class against the number of times those fields are used by the methods within the class.

Cohesion is a percentage.  Actually (or near) zero means that the class has (potentially) low cohesion because the methods make little use of the fields within the class.  Higher numbers mean that the methods make more use of the fields, implying that the class has greater cohesion.

For more information, see this discussion: http://stackoverflow.com/questions/10830135/what-is-high-cohesion-and-how-to-use-it-make-it

## Using the results

The results are a suggestion of where future pain-points may lie in your codebase.  Anything with a very low cohesion or a very high coupling is worth checking over and asking yourself:

Coupling: "Are other objects making too much direct use of this class?"
Cohesion: "Is this class doing one thing well, or is it doing multiple things and needs breaking up?"

Some results will return as 'Not applicable'.  This means that the class has no relevant fields, so it is not possible to make a guess at its cohesion using this measurement.