from typing import List

from pydantic import BaseModel, ConfigDict, Field
from pydantic.alias_generators import to_snake

from dtos.categorys import CategoryDto
from dtos.tags import TagDto
from dtos.users import UserDto


class BlogDto(BaseModel):
    model_config = ConfigDict(
        from_attributes=True,
        alias_generator=to_snake,
        populate_by_name=True,
    )

    Id: int

    Title: str

    Content: str

    Categorys_: CategoryDto = Field(alias="category")

    Users_: UserDto = Field(alias="owner")

    Tags_: List[TagDto] = Field(alias="tags")


class CreateBlogReq(BaseModel):
    title: str
    content: str
    category: str
    tags: List[str]
