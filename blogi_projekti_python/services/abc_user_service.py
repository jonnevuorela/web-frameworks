
import abc

import models



class ABCUserService(abc.ABC):
    @abc.abstractmethod
    def register(self, username: str, password:str) -> models.Users:
        raise NotImplementedError() # pragma: no cover
