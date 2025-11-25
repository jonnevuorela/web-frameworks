from typing import List

from sqlalchemy.orm import Query, Session

import models
from services.abc_blog_service import ABCBlogService


class BlogService(ABCBlogService):
    def __init__(self, _repository: Session):
        self._repository = _repository

    def get_all(self) -> List[models.Blogs]:
        query: Query = self._repository.query(models.Blogs)
        result = query.all()
        return result

    def get_by_id(self, _id: int) -> List[models.Blogs]:
        return self._repository.query(models.Blogs).filter_by(Id=_id).first()
