import abc
from typing import List

import dtos.categorys
import models


class ABCCategoryService(abc.ABC):
    @abc.abstractmethod
    def get_all(self) -> list[models.Categorys]:
        raise NotImplementedError()  # pragma: no cover

    @abc.abstractmethod
    def get_by_id(self, _id: int) -> models.Categorys:
        raise NotImplementedError()  # pragma: no cover

    @abc.abstractmethod
    def create(
        self, req: dtos.categorys.CreateCategoryReq, logged_in_user_id: int
    ) -> models.Categorys:
        raise NotImplementedError()  # pragma: no cover

    @abc.abstractmethod
    def remove(self, _id: int):
        raise NotImplementedError()  # pragma: no cover
