// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open System.IO
open System.Diagnostics
open longVars

module HangmanGame =

    // Finds a text file in project directory
    let countriesFileName = "countries_and_capitals.txt"
    let sourceDirectory = __SOURCE_DIRECTORY__
    let countriesWithCapital = File.ReadAllLines(sourceDirectory + "\\" + countriesFileName)

    // filters the
    let europeans = 
        [
            for line in countriesWithCapital do
            // choose country part
            let country = line.Substring(0, line.IndexOf('|')).Trim()
            // if it's european country, add it to the list of (country * capital) tuples
            if List.contains country europeanCountries then 
                let arrOfTwo = line.Split([|'|'|], 2)
                yield arrOfTwo.[0].Trim() , arrOfTwo.[1].Trim()
        ]

    // Choose random country with capital city in tuple
    let randomCapital (countries:(string * string) list) = 
        let rand = Random().Next(countries.Length)
        countries.[rand]

    // printer for displaying the hidden answer with showed correctly guessed letters
    let hiddenAnswer correctLetters (answer: char[]) =
        for answerLetter in answer do
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

    // Check whether player wants to input - letter of whole word in the loop
    let rec isGuessingTheLetter () = 
        match Console.ReadKey(true).KeyChar with
        | 'l' | 'L' -> true
        | 'w' | 'W' -> false
        | _ -> isGuessingTheLetter () 

    // Adds method for making the char uppercase
    type Char with
        member this.ToUpper() = this.ToString().ToUpper().[0]


    // THE RUNTIME OF THE GAME - loop of the rounds
    let rec hangman answer actualLives round correctLetters wrongLetters = 
        printfn ""
        printfn "---------- ROUND %i ----------" round
        printfn ""
        printLives(actualLives)
        printfn ""
        printfn "Used letters: %A" wrongLetters
        printfn ""
        printf "Secret word: " 
        hiddenAnswer correctLetters answer
        printfn ""
        printfn ""
        printfn "You want to guess the letter or the whole capital name? Type l or w:"
        match isGuessingTheLetter () with
        | true ->
            printfn "Now, guess the letter: "
            match Console.ReadKey().KeyChar.ToUpper() with
            | guessedLetter when Array.contains (guessedLetter) (answer) ->
                if Array.forall ( fun char -> List.contains char (guessedLetter :: correctLetters) ) (answer) 
                // all letters are correctly guessed
                then true
                // correct guess, the game continues
                else hangman answer (actualLives) (round + 1) (guessedLetter :: correctLetters) wrongLetters
            | _ when actualLives = 1 ->
                // wrong guess, the game is lost
                false
            | guessedLetter -> 
                hangman answer (actualLives - 1) (round + 1) correctLetters (guessedLetter :: wrongLetters)
        | false -> 
            printfn "So, you know what do I think? Don't push yourself, we'll hang on. "
            match Console.ReadLine() with
            | guessedWord when Array.forall ( fun ch -> Array.contains ch (answer) ) (guessedWord.ToUpper().ToCharArray()) ->
                true
            | _ when actualLives <= 2 ->
                    false
            | _ -> 
                hangman answer (actualLives - 2) (round + 1) correctLetters wrongLetters

    // START OF THE GAME - all tries loop
    let rec play (guessingTries:int) =
        printfn "Welcome in The Hangman Game!"
        printfn "Try to guess the european country capital city we have in mind. "
        printfn "You have 5 lives. First, you decide whether you want to guess a single letter or the whole answer typing l or w accordingly."
        printfn "If you mistake guessing the letter, you lose one life. If you guessing the whole capital, you lose two lives."
        // Start to measure the elapsed time
        let stoper = Stopwatch.StartNew()

        // remember to make all letters in the answer uppercase in hangman function call
        let country, capital = randomCapital (europeans)
        let hasWon = hangman (capital.ToUpper().ToCharArray()) 5 1 [] []

        // Has player won or lost
        if hasWon then 
            printfn ""
            printfn "Congratulations! You guessed the correct answer."
            printfn ""
            printfn "   %s, the capital city of %s" (capital.ToUpper()) (country.ToUpper())
            printfn ""
            stoper.Stop()
            let elapsedTime = stoper.Elapsed
            let formattedTime = sprintf "%02i:%02i:%02i.%03i" elapsedTime.Hours elapsedTime.Minutes elapsedTime.Seconds elapsedTime.Milliseconds
            printfn "You have completed the game in %f seconds" (elapsedTime.TotalSeconds)
            printfn "Please, enter your name:"
            let playerName = Console.ReadLine()
            printfn ""
            printfn ""

