import traceback
from os import name
from typing import override

from sqlalchemy.orm import Query, Session

import dtos.categorys
import models
from custom_exceptions.not_found_exception import NotFoundException
from services.abc_category_service import ABCCategoryService


class CategoryService(ABCCategoryService):
    def __init__(self, _repository: Session):
        self._repository = _repository

    @override
    def get_all(self) -> list[models.Categorys]:
        query: Query[models.Categorys] = self._repository.query(models.Categorys)
        result = query.all()
        return result

    @override
    def get_by_id(self, _id: int) -> models.Categorys:
        return self._repository.query(models.Categorys).filter_by(id=_id).first()

    @override
    def create(
        self, req: dtos.categorys.CreateCategoryReq, logged_in_user_id: int
    ) -> models.Categorys:
        try:
            category = models.Categorys(
                name=req.name,
                owner_id=logged_in_user_id,
            )
            self._repository.add(category)
            self._repository.commit()
            return category
        except:
            traceback.print_exc()
            raise Exception("Failed to create a new category.")

    @override
    def remove(self, _id: int):
        category = self.get_by_id(_id)
        if category is None:
            raise NotFoundException("category not found")
        self._repository.delete(category)
        self._repository.commit()
