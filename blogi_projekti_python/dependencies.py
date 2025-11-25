import traceback

from fastapi import Depends, HTTPException
from fastapi.security import HTTPAuthorizationCredentials, HTTPBearer

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
