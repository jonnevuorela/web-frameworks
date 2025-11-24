from typing import List

from sqlalchemy import Column, ForeignKey, Index, Integer, LargeBinary, Text, text
from sqlalchemy.orm import Mapped, declarative_base, mapped_column, relationship
from sqlalchemy.orm.base import Mapped

Base = declarative_base()


class Users(Base):
    __tablename__ = 'Users'
    __table_args__ = (
        Index('IX_Users_UserName', 'UserName', unique=True),
    )

    Id = mapped_column(Integer, primary_key=True)
    HashedPassword = mapped_column(LargeBinary, nullable=False)
    PasswordSalt = mapped_column(LargeBinary, nullable=False)
    Role = mapped_column(Text, nullable=False)
    UserName = mapped_column(Text, nullable=False)
    Xp = mapped_column(Integer, nullable=False, server_default=text('0'))

    Blogs: Mapped[List['Blogs']] = relationship('Blogs', uselist=True, back_populates='Users_')


class Blogs(Base):
    __tablename__ = 'Blogs'
    __table_args__ = (
        Index('IX_Blogs_AppUserId', 'AppUserId'),
    )

    Id = mapped_column(Integer, primary_key=True)
    Title = mapped_column(Text, nullable=False)
    Content = mapped_column(Text, nullable=False)
    AppUserId = mapped_column(ForeignKey('Users.Id', ondelete='CASCADE'), nullable=False)

    Users_: Mapped['Users'] = relationship('Users', back_populates='Blogs')
