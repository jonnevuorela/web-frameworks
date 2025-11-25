from typing import Annotated, List

from fastapi import APIRouter, Depends, HTTPException

import dtos.blogs
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
