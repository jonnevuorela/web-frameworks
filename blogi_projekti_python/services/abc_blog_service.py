import abc
from typing import List

import models


class ABCBlogService(abc.ABC):
    @abc.abstractmethod
    def get_all(self) -> List[models.Blogs]:
        raise NotImplementedError()  # pragma: no cover
