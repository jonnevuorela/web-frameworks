from pydantic import BaseModel


class RegisterReq(BaseModel):
    username: str
    password: str


class RegisterRes(BaseModel):
    id: int
    username: str
    role: str
