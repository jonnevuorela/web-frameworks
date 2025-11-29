from typing import Annotated

from fastapi import APIRouter, Depends, HTTPException

import dtos.categorys
import models
from custom_exceptions.not_found_exception import NotFoundException
from dependencies import get_logged_in_user
from factories.services import category_service_factory
from services.abc_category_service import ABCCategoryService

router = APIRouter(prefix="/api/categorys", tags=["categorys"])

CategoryServ = Annotated[ABCCategoryService, Depends(category_service_factory)]


@router.get("/")
async def get_all_categorys(
    _category_service: CategoryServ,
) -> list[dtos.categorys.CategoryDto]:
    try:
        categorys = _category_service.get_all()

        return categorys  # pyright: ignore[reportReturnType]
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))


@router.get("/{category_id}")
async def get_category_by_id(
    category_id: int, _category_service: CategoryServ
) -> dtos.categorys.CategoryDto:
    try:
        category = _category_service.get_by_id(category_id)
        if category is None:
            raise HTTPException(status_code=404, detail="category not found")
        return category
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))


@router.post("/")
async def create_category(
    req: dtos.categorys.CreateCategoryReq,
    _category_service: CategoryServ,
    logged_in_user: models.Users = Depends(get_logged_in_user),
) -> dtos.categorys.CategoryDto:
    try:
        category = _category_service.create(req, logged_in_user.Id)
        return category
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))


@router.delete("/{category_id}")
async def remove_category(category_id: int, _category_service: CategoryServ):
    try:
        _category_service.remove(category_id)
        return ""

    except NotFoundException as error:
        raise HTTPException(status_code=404, detail=str(error))
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))
