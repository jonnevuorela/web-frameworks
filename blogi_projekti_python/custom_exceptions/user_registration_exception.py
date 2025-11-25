class UserRegistrationException(Exception):
    def __init__(self, message: str = "error registering user") -> None:
        super(Exception, self).__init__(message)
