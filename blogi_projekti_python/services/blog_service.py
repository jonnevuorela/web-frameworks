from typing import List

from sqlalchemy.orm import Query, Session

import dtos.blogs
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

    def create(
        self, req: dtos.blogs.CreateBlogReq, logged_in_user_id: int
    ) -> models.Blogs:
        blog = models.Blogs(
            Title=req.title, Content=req.content, AppUserId=logged_in_user_id
        )
        self._repository.add(blog)
        self._repository.commit()
        return blog
