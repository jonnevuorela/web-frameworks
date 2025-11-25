class ForbiddenException(Exception):
    def __init__(self, message: str = "forbidden") -> None:
        super(Exception, self).__init__(message)
