from typing import Annotated, List

from fastapi import APIRouter, Depends, HTTPException

import dtos.login
import dtos.register
import dtos.users
from custom_exceptions.not_found_exception import NotFoundException
from dependencies import require_admin, require_xp
from factories.services import user_service_factory
from services.abc_user_service import ABCUserService

router = APIRouter(prefix="/api/users", tags=["users"])

UserServ = Annotated[ABCUserService, Depends(user_service_factory)]


@router.post("/register")
async def register(
    request_data: dtos.register.RegisterReq, _user_service: UserServ
) -> dtos.register.RegisterRes:
    user = _user_service.register(request_data.username, request_data.password)
    return dtos.register.RegisterRes(
        id=user.Id,
        username=user.UserName,
        role=user.Role,
    )


@router.post("/login")
async def login(
    request_data: dtos.login.LoginReq, _user_service: UserServ
) -> dtos.login.LoginRes:
    try:
        token = _user_service.login(request_data.username, request_data.password)
        return dtos.login.LoginRes(token=token)
    except NotFoundException:
        raise HTTPException(status_code=404, detail="Username or password incorrect")
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))


@router.get("/", dependencies=[Depends(require_admin)])
async def get_all_users(_user_service: UserServ) -> List[dtos.users.UserDto]:
    try:
        users = _user_service.get_all()

        # return [dtos.users.UserDto.model_validate(user) for user in users]
        return users
    except Exception as error:
        raise HTTPException(status_code=500, detail=str(error))


@router.get("/account/rewards", dependencies=[Depends(require_xp(1000))])
async def get_user_rewards():
    return "hello"
