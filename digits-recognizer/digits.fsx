﻿// This F# exercise is directly inspired by the 
// Digit Recognizer competition from Kaggle.com:
// http://www.kaggle.com/c/digit-recognizer
// The datasets below are simply shorter versions of
// the training dataset from Kaggle.
 
// The goal of the exercise will be to
// create a classifier that uses training data
// to recognize hand-written digits, and
// evaluate the quality of our classifier
// by looking at predictions on the validation data.

// This file provides some guidance through the problem:
// each section is numbered, and 
// solves one piece you will need. Sections contain
// general instructions, 
// [ YOUR CODE GOES HERE! ] tags where you should
// make the magic happen, and
// <F# QUICK-STARTER> blocks. These are small
// F# tutorials illustrating aspects of the
// syntax which could come in handy. Run them,
// see what happens, and tweak them to fit your goals!

  
// 0. GETTING READY
 

// <F# QUICK-STARTER> 
// With F# Script files (.fsx) and F# Interactive,
// you can "live code" and see what happens.

// Try typing let x = 42 in the script file, 
// select the code and press Alt+Enter.
// (in VS, right-click & select "Execute in interactive")

let x = 42

// let "binds" the value on the right to a name.

// Try now typing x + 3;; in the F# Interactive window.
// ';;' indicates "execute now whatever I just typed".

// Now right-click the following 2 lines and execute:
let greet name = 
    printfn "Hello, %s" name

greet "Malaga"

// let also binds a name to a function.
// greet is a function with one argument, name.
// You should be able to run this in F# Interactive:
// greet "World";;
// </F# QUICK-STARTER> 

// Two data files are included in the same place you
// found this script: 
// trainingsample.csv, a file that contains 5,000 examples, and 
// validationsample.csv, a file that contains 500 examples.
// The first file will be used to train your model, and the
// second one to validate the quality of the model.
 
// 1. GETTING SOME DATA
 
// First let's read the contents of "trainingsample.csv"

// We will need System and System.IO to work with files,
// let's right-click / run in interactive, 
// to have these namespaces loaded:
  
open System
open System.IO

// the following might come in handy: 
// File.ReadAllLines(path)
// returns an array of strings for each line 

let localPath = __SOURCE_DIRECTORY__
let dataPath = Path.Combine(localPath,"trainingsample.csv")

let dataLines = File.ReadAllLines(dataPath)
 
// 2. EXTRACTING COLUMNS
 
// Break each line of the file into an array of string,
// separating by commas, using Array.map

// <F# QUICK-STARTER> 
// Array.map quick-starter:
// Array.map takes an array, and transforms it
// into another array by applying a function to it.
// Example: starting from an array of strings:
let strings = [| "Machine"; "Learning"; "with"; "F#"; "is"; "fun" |]

// We can transform it into a new array,
// containing the length of each string:
let lengths = Array.map (fun (s:string) -> s.Length) strings
// The exact same operation above can be 
// done using the forward pipe operator, 
// which makes it look nicer:
let containsI = strings |> Array.map (fun s -> s.Contains("i"))
// </F# QUICK-STARTER> 
 
// The following function might help
let csvToSplit = "1,2,3,4,5"
let splitResult = csvToSplit.Split(',')
 
let dataMatrix = dataLines |> Array.map (fun line -> line.Split(',')) 
 
 
// 3. CLEANING UP HEADERS
 
// Did you note that the file has headers? We want to get rid of it.

// <F# QUICK-STARTER>  
// Array slicing quick starter:
// Let's start with an Array of ints:
let someNumbers = [| 0 .. 10 |] // create an array from 0 to 10
// You can access Array elements by index:
let first = someNumbers.[0] 
// You can also slice the array:
let twoToFive = someNumbers.[ 1 .. 4 ] // grab a slice
let upToThree = someNumbers.[ .. 2 ]

someNumbers.[ 1 .. ] 
// </F# QUICK-STARTER> 

let dataMatrixWithoutHeaders = dataMatrix.[ 1 .. ]
 
 
// 4. CONVERTING FROM STRINGS TO INTS
 
// Now that we have an array containing arrays of strings,
// and the headers are gone, we need to transform it 
// into an array of arrays of integers.
// Array.map seems like a good idea again :)

// The following might help:
let castedInt = (int)"42"
// or, alternatively:
let convertedInt = Convert.ToInt32("42")
// or, even more simple:
let intAsFunction = int "42" 
 
let numbersMatrix = dataMatrixWithoutHeaders |> Array.map(fun line -> line |> Array.map(int)) 
 
// 5. CONVERTING ARRAYS TO RECORDS
 
// Rather than dealing with a raw array of ints,
// for convenience let's store these into an array of Records

// <F# QUICK-STARTER>  
// Record quick starter: we can declare a 
// Record (a lightweight, immutable class) type that way:
type Example = { Label:int; Pixels:int[] }
// and instantiate one this way:
let example = { Label = 1; Pixels = [| 1; 2; 3; |] }

// Pann: records are distinguished by prop names
// if you create another type with the same props, it overrides the old one
type MyExample = { Label:int; PixelsCount:int }

let myExample = { MyExample.Label = 1; PixelsCount = 1 }

// </F# QUICK-STARTER>  

let records = numbersMatrix |> Array.map(fun line -> { Label = line.[ 0 ]; Pixels = line.[ 1 .. ] })

let RemoveHeaderLine (lines: string[]) = lines.[ 1 .. ]

let CsvSplit (line: string) = line.Split(',')

let CreateRecord (ints: int[]) =
    { Label = ints.[ 0 ]; Pixels = ints.[ 1 .. ] }

let ReadRecords (filename: string) =
    Path.Combine(localPath,filename)
    |> File.ReadAllLines
    |> RemoveHeaderLine
    |> Array.map (CsvSplit >> Array.map (int) >> CreateRecord)

let knownRecords = ReadRecords "trainingsample.csv"
// 6. COMPUTING DISTANCES
 
// We need to compute the distance between images
// Math reminder: the euclidean distance is
// distance [ x1; y1; z1 ] [ x2; y2; z2 ] = 
// sqrt((x1-x2)*(x1-x2) + (y1-y2)*(y1-y2) + (z1-z2)*(z1-z2))
 
// <F# QUICK-STARTER> 
// Array.map2 could come in handy here.
// Suppose we have 2 arrays:
let point1 = [| 0; 1; 2 |]
let point2 = [| 3; 4; 5 |]
// Array.map2 takes 2 arrays at a time
// and maps pairs of elements, for instance:
let map2Example = 
    Array.map2 (+) point1 point2
// This simply computes the sums for point1 and point2,
// but we can easily turn this into a function now:
let map2PointsExample (P1: int[]) (P2: int[]) =
    Array.map2 (+) P1 P2
// </F# QUICK-STARTER>  


let squarediff a b = pown (a - b) 2

// Having a function like
let distance (p1: int[]) (p2: int[]) =
    (p1, p2)
    ||> Array.map2 squarediff  
    |> Array.sum 
    |> float 
    |> sqrt
    
// would come in very handy right now,
// except that in this case, 
// 42 is likely not the right answer
 
distance [| 0; 0; 0 |] [| 3; 3; 3 |]
 
// 7. WRITING THE CLASSIFIER FUNCTION
 
// We are now ready to write a classifier function!
// The classifier should take a set of pixels
// (an array of ints) as an input, search for the
// closest example in our sample, and predict
// the value of that closest element.
 
// <F# QUICK-STARTER> 
// Array.minBy can be handy here, to find
// the closest element in the Array of examples.
// Suppose we have an Array of Example:
let someData = 
    [| { Label = 0; Pixels = [| 0; 1 |] };
       { Label = 1; Pixels = [| 9; 2 |] };
       { Label = 2; Pixels = [| 3; 4 |] }; |]
// We can find for instance 
// the element with largest first pixel
let findThatGuy = 
    someData 
    |> Array.maxBy (fun x -> x.Pixels.[0])
// </F# QUICK-STARTER> 

 
// <F# QUICK-STARTER> 
// F# and closures work very well together
let immutableValue = 42
let functionWithClosure (x: int) =
    if x > immutableValue // using outside value
    then true
    else false
// </F# QUICK-STARTER>  
 

 // The classifier function should probably
// look like this - except that this one will
// classify everything as a 0:
  
let classify (classifier: int[] -> int[] -> float) (unknown:int[]) =
    knownRecords
    |> Array.minBy (fun known -> classifier unknown known.Pixels)
    |> fun bestMatch -> bestMatch.Label

let classifyByDistance =
    classify distance

let flatDistance (p1: int[]) (p2: int[]) =
    (p1, p2)
    ||> Array.map2 (fun a b -> abs (a - b))
    |> Array.sum
    |> float

let classifyByFlatDistance =
    classify flatDistance

// 8. EVALUATING THE MODEL AGAINST VALIDATION DATA
 
// Now that we have a classifier, we need to check
// how good it is. 
// This is where the 2nd file, validationsample.csv,
// comes in handy. 
// For each Example in the 2nd file,
// we know what the true Label is, so we can compare
// that value with what the classifier says.
// You could now check for each 500 example in that file
// whether your classifier returns the correct answer,
// and compute the % correctly predicted.

let validationRecords = ReadRecords ("validationsample.csv")

let calculateCorrectness (classifier: int[] -> int[] -> float) =
    validationRecords
    |> Array.filter (fun record -> (classify classifier) record.Pixels = record.Label)
    |> fun correctRecords -> float correctRecords.Length / float validationRecords.Length

let correctnessDistance =
    calculateCorrectness distance

let correctnessFlatDistance =
    calculateCorrectness flatDistance

    // |> fun correctRecords -> correctRecords.Length
    // |> float
    // |> fun correctLenth -> correctLenth / float validationRecords.Length

// 9. SHIP IT!

// By then, you should have a running classifier, which
// recognizes reasonably well hand-written digits.
// What can you do next?

// Improve accuracy: 94.4% is not bad, but it's not 100%.
// Try out model variations, for instance:
// * instead of 1 neighbor, take k neighborsw and predict
// the majority label
// * use a different distance, for instance abs (x - y)
// * reduce / rescale the pixels from 0 - 255 to 0 - k
// * apply blurring to pixels
// * ... try out completely different algorithms?

// Improve speed: this algorithm is not very fast. Can 
// you squeeze out some speed? 
// Look into Array.Parallel. ...
// Use #time to tune your code

// Try to enter the real Kaggle competition?
