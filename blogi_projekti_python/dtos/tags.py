from pydantic import BaseModel, ConfigDict
from pydantic.alias_generators import to_snake


class TagDto(BaseModel):
    model_config = ConfigDict(
        from_attributes=True,
        alias_generator=to_snake,
        populate_by_name=True,
    )
    Id: int
    TagText: str
