# Project Overview

We have developed a quiz application where users can create new quiz packs, edit questions in existing packs, and import quiz packs from an external source. Through the application's navigation, users can switch between managing their quiz packs and playing quizzes.

## Technical Implementation

### Project Structure and MVVM

We built the application using the MVVM pattern. The Views handle the user interface, the ViewModels handle the application logic and functionality, and the Models represent the data used in the application.

The application uses a shared `ViewModelBase` class with functionality such as `SetProperty` and `RaisePropertyChanged`, allowing WPF to update the UI when properties change. The `MainWindowViewModel` acts as the main DataContext and holds references to the other ViewModels, making it possible for different parts of the application to communicate.

### Navigation

Navigation is handled through a shared `ChangeView` method, with separate methods for showing `ConfigurationViewModel` and `PlayerViewModel`. This gave me more control over navigation, particularly when creating new question packs from different views.

### Saving and Loading Question Packs

The application uses JSON to save and load question packs. When loading, the JSON data is deserialized into `QuestionPack` objects and validated to ensure that each question has one correct answer and three incorrect answers. Invalid questions are skipped while valid questions from the same pack are kept. When the application closes, the current question packs are saved back to the JSON file.

If loading fails, the application can still start with an empty collection of question packs, allowing the user to continue using the application. When saving, the user can choose whether to overwrite the existing file or save the question packs to a new file.

### External API Integration

The application allows users to import questions from the Open Trivia Database. First, the application retrieves the available categories, which the user can select along with the desired difficulty and number of questions. When importing, the application requests questions based on these selections.

Only multiple-choice questions are requested. The imported questions are also validated when loaded to ensure they have the structure required by the application: one correct answer and three incorrect answers.

### Error Handling

I have added error handling to protect the application when working with external data. The application handles potential null values in its regular functionality, while operations involving JSON files and API requests use exception handling. This includes handling errors such as invalid JSON data, file access problems and invalid arguments, helping the application recover from these problems instead of crashing unexpectedly.

## Development Challenges

### Handling Duplicate Answers

One of the biggest challenges I encountered during development was handling duplicate answers in the quiz.

My initial implementation used a list and `IndexOf()`, which worked for basic functionality but could not distinguish between two answers with identical text.

To handle this, I used `MultiDataTriggers` in WPF to control how the answer buttons are presented. These triggers check multiple conditions, including the selected answer index and whether the answer is correct, to determine how each button should be displayed.

I also used tuples to store each answer together with a boolean value indicating whether it was correct. This allowed the application to distinguish between correct and incorrect answers even when multiple options contained identical text.

### Handling JSON Loading Errors

Another challenge I encountered was deciding how to handle the original JSON file when loading failed.

There were several possible ways to approach this, but I wanted to make sure the user could still open the application even if the JSON file was corrupted or an exception occurred. I implemented a message box that allows the user to either continue with an empty collection of question packs or try loading the file again.

However, this introduced another problem. If the original file could not be loaded, I did not want the user to accidentally overwrite it and potentially lose previously saved data. I therefore added the option to either overwrite the existing file or save the question packs to a new location.

This required me to learn more about file handling and how to let users choose where their data should be saved.
