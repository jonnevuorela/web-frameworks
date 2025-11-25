import abc

import models


class ABCTokenTool(abc.ABC):
    @abc.abstractmethod
    def create_token(self, user: models.Users) -> str:
        raise NotImplementedError()  # pragma: no cover

    @abc.abstractmethod
    def decode_token(self, token: str) -> dict | None:
        raise NotImplementedError()  # pragma: no cover
