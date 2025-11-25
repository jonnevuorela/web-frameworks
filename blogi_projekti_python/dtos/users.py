from pydantic import BaseModel, ConfigDict
from pydantic.alias_generators import to_snake


class UserDto(BaseModel):
    model_config = ConfigDict(
        from_attributes=True,
        alias_generator=to_snake,
        populate_by_name=True,
    )

    # Id -> id
    Id: int
    # UserName -> user_name
    UserName: str
    # Role -> role
    Role: str
