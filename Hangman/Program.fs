// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open System.IO
open System.Diagnostics

module HangmanGame =

    // Finds a text file in project directory
    let countriesFileName = "countries_and_capitals.txt"
    let sourceDirectory = __SOURCE_DIRECTORY__
    let countriesWithCapital = File.ReadAllLines(sourceDirectory + "\\" + countriesFileName)

    // filters the
    let europeans = 
        Array.toList countriesWithCapital
        // filter for only european countries with respect to "europeanCountries" list
        |> List.filter (
            fun line -> 
                let country = line.Substring(0, line.IndexOf('|')).Trim()
                List.contains country europeanCountries) 
        // Divide all string lines into a tuple
        |> List.map (
            fun pair ->
                let arrOfTwo = pair.Split([|'|'|], 2)
                arrOfTwo.[0].Trim() , arrOfTwo.[1].Trim()
                )

    printf "%A" europeans

    // VARIABLES, METHODS, FLAGS

    // Choose random country with capital city in tuple
    let randomCapital (countries:(string * string) list) = 
        let rand = Random().Next(countries.Length)
        countries.[rand]

    // printer for displaying the hidden answer with showed correctly guessed letters
    let hiddenAnswer correctLetters (answer:string) =
        for answerLetter in answer.ToCharArray() do
            if List.contains answerLetter correctLetters
            then printf " %c" answerLetter
            else printf " _"

    // Ask player whether he/she wants to play again
    let rec askForAgain () =
        printfn "Do you want to play again? [Y/N]"
        match Console.ReadKey(true).KeyChar with
        | 'y' | 'Y' -> true 
        | 'n' | 'N' -> false
        | _ -> askForAgain ()
