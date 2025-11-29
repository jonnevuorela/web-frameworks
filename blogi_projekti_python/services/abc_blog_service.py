import abc
from typing import List

import dtos.blogs
import models


class ABCBlogService(abc.ABC):
    @abc.abstractmethod
    def get_all(self) -> List[models.Blogs]:
        raise NotImplementedError()  # pragma: no cover

    @abc.abstractmethod
    def get_by_id(self, _id: int) -> models.Blogs:
        raise NotImplementedError()  # pragma: no cover

    @abc.abstractmethod
    def create(
        self, req: dtos.blogs.CreateBlogReq, logged_in_user_id: int
    ) -> models.Blogs:
        raise NotImplementedError()  # pragma: no cover

    @abc.abstractmethod
    def remove(self, _id: int):
        raise NotImplementedError()  # pragma: no cover
