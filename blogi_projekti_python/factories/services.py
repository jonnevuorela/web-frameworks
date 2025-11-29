from fastapi import Depends
from sqlalchemy.orm import Session

from db import get_db
from services.blog_service import BlogService
from services.category_service import CategoryService
from services.user_service import UserService
from tools.abc_token_tool import ABCTokenTool
from tools.symmetric_token_tool import SymmetricTokenTool


def token_tool_factory():
    return SymmetricTokenTool()


def user_service_factory(
    repository: Session = Depends(get_db),
    token_tool: ABCTokenTool = Depends(token_tool_factory),
):
    return UserService(repository, token_tool)


def blog_service_factory(repository: Session = Depends(get_db)):
    return BlogService(repository)


def category_service_factory(repository: Session = Depends(get_db)):
    return CategoryService(repository)
