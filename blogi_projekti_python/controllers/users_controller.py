
from typing import Annotated

from fastapi import APIRouter, Depends

import dtos.register

from factories.services import user_service_factory
from services.abc_user_service import ABCUserService

router = APIRouter(prefix='/api/users', tags=['users'])

UserServ = Annotated[ABCUserService, Depends(user_service_factory)]

@router.post('/register')
async def register(request_data: dtos.register.RegisterReq,
                   _user_service: UserServ) -> dtos.register.RegisterRes:

    user = _user_service.register(request_data.username, request_data.password)

    return dtos.register.RegisterRes(
        id=user.Id,
        username=user.UserName,
        role=user.Role,
    )
