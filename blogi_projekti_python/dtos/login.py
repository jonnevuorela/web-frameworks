from pydantic import BaseModel


class LoginReq(BaseModel):
    username: str
    password: str


class LoginRes(BaseModel):
    token: str
