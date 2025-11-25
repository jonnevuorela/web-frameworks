import traceback

from fastapi import Depends, HTTPException
from fastapi.security import HTTPAuthorizationCredentials, HTTPBearer

import models
from custom_exceptions.forbidden_exception import ForbiddenException
from factories.services import user_service_factory
from services.abc_user_service import ABCUserService

oauth2_scheme = HTTPBearer()


def get_logged_in_user(
    authorization: HTTPAuthorizationCredentials = Depends(oauth2_scheme),
    _user_service: ABCUserService = Depends(user_service_factory),
):
    try:
        user = _user_service.get_user_by_access_token(authorization.credentials)
        if user is None:
            raise HTTPException(status_code=401, detail="Unauthorized")
        return user
    except Exception:
        traceback.print_exc()
        raise HTTPException(status_code=401, detail="Unauthorized")


def require_admin(
    authorization: HTTPAuthorizationCredentials = Depends(oauth2_scheme),
    _user_service: ABCUserService = Depends(user_service_factory),
):
    try:
        user = get_logged_in_user(authorization, _user_service)
        if user.Role != "admin":
            raise ForbiddenException()
    except ForbiddenException:
        raise HTTPException(status_code=403, detail="Forbidden")

    except Exception:
        raise HTTPException(status_code=401, detail="Unauthorized")


def require_xp(required_xp: int):
    def check_xp(logged_in_user: models.Users = Depends(get_logged_in_user)):
        if logged_in_user.Xp < required_xp:
            raise HTTPException(status_code=403, detail="xp too low")

    return check_xp
