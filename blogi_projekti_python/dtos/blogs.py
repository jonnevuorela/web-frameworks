from pydantic import BaseModel, ConfigDict, Field
from pydantic.alias_generators import to_snake

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

    Users_: UserDto = Field(alias="owner")
