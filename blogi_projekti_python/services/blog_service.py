import traceback
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

        try:
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
        except:
            traceback.print_exc
            raise Exception("Handling the tags on blog creation failed")

        try:
            category: models.Categorys
            existing_category_qry: Query = self._repository.query(
                models.Categorys
            ).filter(models.Categorys.name == req.category)
            if (existing_category := existing_category_qry.first()) is not None:
                print(
                    "did not create new category on blog creation",
                    "\nnimi: ",
                    existing_category.name,
                    "\nid: ",
                    existing_category.id,
                    "\nowner_id: ",
                    existing_category.owner_id,
                )
                category = existing_category
            else:
                new_category = models.Categorys(
                    name=req.category,
                    owner_id=logged_in_user_id,
                )
                self._repository.add(new_category)
                self._repository.commit()
                print(
                    "created new category on blog creation",
                    "\nnimi: ",
                    new_category.name,
                    "\nid: ",
                    new_category.id,
                    "\nowner_id: ",
                    new_category.owner_id,
                )
                category = new_category
        except:
            traceback.print_exc
            raise Exception("Handling the categorys on blog creation failed")

        try:
            blog = models.Blogs(
                Title=req.title,
                Content=req.content,
                CategoryId=category.id,
                AppUserId=logged_in_user_id,
                Tags_=all_tags,
            )
            self._repository.add(blog)
            self._repository.commit()
        except:
            traceback.print_exc
            raise Exception("Handling the blogs on blog creation failed")

        return blog

    def remove(self, _id: int):
        blog = self.get_by_id(_id)
        if blog is None:
            raise NotFoundException("blog not found")
        self._repository.delete(blog)
        self._repository.commit()
