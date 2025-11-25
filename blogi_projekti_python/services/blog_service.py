from typing import List

from sqlalchemy.orm import Query, Session

import dtos.blogs
import models
from custom_exceptions.not_found_exception import NotFoundException
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

        existing_tags_qry: Query = self._repository.query(models.Tags).filter(
            models.Tags.TagText.in_(req.tags)
        )
        existing_tags: List[models.Tags] = existing_tags_qry.all()
        existing_tag_texts = [tag.TagText for tag in existing_tags]
        new_tag_texts = list(set(req.tags) - set(existing_tag_texts))

        new_tags: List[models.Tags] = []
        for text in new_tag_texts:
            _new_tag = models.Tags(TagText=text)

            new_tags.append(_new_tag)

        self._repository.add_all(new_tags)
        self._repository.commit()

        all_tags = existing_tags + new_tags

        blog = models.Blogs(
            Title=req.title,
            Content=req.content,
            AppUserId=logged_in_user_id,
            Tags_=all_tags,
        )
        self._repository.add(blog)
        self._repository.commit()
        self._repository.add(blog)
        self._repository.commit()
        return blog

    def remove(self, _id: int):
        blog = self.get_by_id(_id)
        if blog is None:
            raise NotFoundException("blog not found")
        self._repository.delete(blog)
        self._repository.commit()
