namespace FindYourStoryApp.Models
{
    //FreeCode Spot (2020) demonstrates the FirebaseError class.
    //This class represents the overall error response from Firebase.
    public class FirebaseError
    {
        public Error error { get; set; }
    }

    //FreeCode Spot (2020) demonstrates the Error class.
    //This class represents a single error within the Firebase error response.
    //Contains detailed information about the error, such as the error code, message, and any additional errors.
    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<Error> errors { get; set; }
    }
}
