namespace WebAPI_Code_First.Utilities
{
    public static class Constants
    {
        public const string SUCCESSMSG = "Success";
        public const string FAILUREMSG = "Operation failed";
        public const string NO_RECORDS_FOUND = "No users found";
        public const string INVALID_REQUEST = "Invalid request data";
        public const string USER_NOT_FOUND = "User not found";
        public const string USER_UPDATED_SUCCESS = "User updated successfully";

        //public const string INVALID_REQUEST = "Invalid request.";
        public const string EMAIL_ALREADY_EXISTS = "Email already exists.";
        public const string CONTACT_ALREADY_EXISTS = "Contact number already exists.";
        public const string EMAIL_AND_CONTACT_ALREADY_EXISTS = "Both email and contact number already exist.";
        public const string USER_REGISTERED_SUCCESS = "User registered successfully.";

        public const int FAILED_OPERATION = 400;
    }
}


//namespace WebAPI_Code_First.Utilities
//{
//    public static class Constants
//    {
//        // Success Messages
//        public const int SUCCESS_CODE = 200;
//        public const string SUCCESSMSG = "Success";
//        public const string RECORDS_FOUND = "Records retrieved successfully";
//        public const string USER_UPDATED = "User updated successfully";
//        public const string USER_CREATED = "User created successfully";
//        public const string OPERATION_SUCCESSFUL = "Operation completed successfully";

//        // Error Messages
//        public const int FAILED_OPERATION = 400;
//        public const string FAILUREMSG = "Operation failed";
//        public const string NO_RECORDS_FOUND = "No records found";
//        public const string INVALID_REQUEST = "Invalid request data";
//        public const string USER_NOT_FOUND = "User not found";
//        public const string EMAIL_ALREADY_EXISTS = "Email ID already exists";
//        public const string USERNAME_ALREADY_EXISTS = "Username already exists";
//        public const string PASSWORD_INCORRECT = "Incorrect password";

//        // Authentication Messages
//        public const int UNAUTHORIZED_ACCESS = 401;
//        public const string UNAUTHORIZED_MSG = "Unauthorized access";
//        public const int FORBIDDEN_ACCESS = 403;
//        public const string FORBIDDEN_MSG = "Access denied";
//        public const string TOKEN_EXPIRED = "Token has expired";
//        public const string TOKEN_INVALID = "Invalid token";

//        // Database & Validation Messages
//        public const int SERVER_ERROR = 500;
//        public const string SERVER_ERROR_MSG = "Internal server error occurred";
//        public const string DATABASE_ERROR = "Database operation failed";
//        public const string REQUIRED_FIELD_MISSING = "One or more required fields are missing";
//        public const string INVALID_DATA = "Invalid input data";

//        // Generic Messages
//        public const string DELETE_SUCCESS = "Record deleted successfully";
//        public const string DELETE_FAILURE = "Failed to delete the record";
//        public const string UPDATE_FAILURE = "Failed to update record";
//        public const string CREATE_FAILURE = "Failed to create record";

//        // Custom Constants
//        public const int RECORD_NOT_FOUND = 404;
//        public const int RECORD_ALREADY_EXISTS = 409;
//        public const string RECORD_ALREADY_EXISTS_MSG = "Record already exists";
//        public const int DATA_CONFLICT = 409;
//        public const string DATA_CONFLICT_MSG = "Data conflict detected";

//        // Miscellaneous
//        public const string PASSWORD_CHANGED = "Password updated successfully";
//        public const string EMAIL_VERIFIED = "Email ID verified";
//        public const string ACCOUNT_LOCKED = "Account is locked due to multiple failed login attempts";
//    }
//}
