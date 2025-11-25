from typing import Annotated, List

from fastapi import APIRouter, Depends, HTTPException

import dtos.blogs
import models
from custom_exceptions.not_found_exception import NotFoundException
from dependencies import get_logged_in_user
from factories.services import blog_service_factory
from services.abc_blog_service import ABCBlogService

router = APIRouter(prefix="/api/blogs", tags=["blogs"])

BlogServ = Annotated[ABCBlogService, Depends(blog_service_factory)]


@router.get("/")
async def get_all_blogs(_blog_service: BlogServ) -> List[dtos.blogs.BlogDto]:
    try:
        blogs = _blog_service.get_all()

        # return [dtos.users.UserDto.model_validate(user) for user in users]
        return blogs
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))


@router.get("/{blog_id}")
async def get_blog_by_id(blog_id: int, _blog_service: BlogServ) -> dtos.blogs.BlogDto:
    try:
        blog = _blog_service.get_by_id(blog_id)
        if blog is None:
            raise HTTPException(status_code=404, detail="blog not found")
        return blog
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))


@router.post("/")
async def create_blog(
    req: dtos.blogs.CreateBlogReq,
    _blog_service: BlogServ,
    logged_in_user: models.Users = Depends(get_logged_in_user),
) -> dtos.blogs.BlogDto:
    try:
        blog = _blog_service.create(req, logged_in_user.Id)
        return blog
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))


@router.delete("/{blog_id}")
async def remove_blog(blog_id: int, _blog_service: BlogServ):
    try:
        _blog_service.remove(blog_id)
        return ""

    except NotFoundException as error:
        raise HTTPException(status_code=404, detail=str(error))
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))
