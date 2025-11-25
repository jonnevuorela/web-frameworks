import abc

import models


class ABCUserService(abc.ABC):
    @abc.abstractmethod
    def register(self, username: str, password: str) -> models.Users:
        raise NotImplementedError()  # pragma: no cover

    @abc.abstractmethod
    def login(self, username: str, password: str) -> str:
        raise NotImplementedError()  # pragrma: no cover

    @abc.abstractmethod
    def get_user_by_username(self, username: str) -> models.Users | None:
        raise NotImplementedError()  # pragma: no cover

    @abc.abstractmethod
    def get_all(self) -> list[models.Users]:
        raise NotImplementedError()  # pragma: no cover
