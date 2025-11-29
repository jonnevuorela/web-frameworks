from typing import List

from sqlalchemy import (
    Column,
    ForeignKey,
    Index,
    Integer,
    LargeBinary,
    Table,
    Text,
    text,
)
from sqlalchemy.orm import Mapped, declarative_base, mapped_column, relationship
from sqlalchemy.orm.base import Mapped

Base = declarative_base()
metadata = Base.metadata


class Tags(Base):
    __tablename__ = "Tags"

    TagText = mapped_column(Text, nullable=False)
    Id = mapped_column(Integer, primary_key=True)

    Blogs: Mapped[List["Blogs"]] = relationship(
        "Blogs", secondary="BlogTag", back_populates="Tags_"
    )


class Users(Base):
    __tablename__ = "Users"
    __table_args__ = (Index("IX_Users_UserName", "UserName", unique=True),)

    HashedPassword = mapped_column(LargeBinary, nullable=False)
    PasswordSalt = mapped_column(LargeBinary, nullable=False)
    Role = mapped_column(Text, nullable=False)
    UserName = mapped_column(Text, nullable=False)
    Xp = mapped_column(Integer, nullable=False, server_default=text("0"))
    Id = mapped_column(Integer, primary_key=True)

    Categorys: Mapped[List["Categorys"]] = relationship(
        "Categorys", uselist=True, back_populates="owner"
    )
    Blogs: Mapped[List["Blogs"]] = relationship(
        "Blogs", uselist=True, back_populates="Users_"
    )


class Categorys(Base):
    __tablename__ = "Categorys"

    id = mapped_column(Integer, primary_key=True, unique=True)
    name = mapped_column(Text, nullable=False, unique=True)
    owner_id = mapped_column(ForeignKey("Users.Id"), nullable=False)

    owner: Mapped["Users"] = relationship("Users", back_populates="Categorys")
    Blogs: Mapped[List["Blogs"]] = relationship(
        "Blogs", uselist=True, back_populates="Categorys_"
    )


class Blogs(Base):
    __tablename__ = "Blogs"
    __table_args__ = (Index("IX_Blogs_AppUserId", "AppUserId"),)

    Title = mapped_column(Text, nullable=False)
    Content = mapped_column(Text, nullable=False)
    AppUserId = mapped_column(
        ForeignKey("Users.Id", ondelete="CASCADE"), nullable=False
    )
    CategoryId = mapped_column(
        ForeignKey("Categorys.id", ondelete="CASCADE"), nullable=False
    )
    Id = mapped_column(Integer, primary_key=True)

    Users_: Mapped["Users"] = relationship("Users", back_populates="Blogs")
    Categorys_: Mapped["Categorys"] = relationship("Categorys", back_populates="Blogs")

    Tags_: Mapped[List["Tags"]] = relationship(
        "Tags", secondary="BlogTag", back_populates="Blogs"
    )


t_BlogTag = Table(
    "BlogTag",
    metadata,
    Column(
        "BlogId",
        ForeignKey("Blogs.Id", ondelete="CASCADE"),
        primary_key=True,
        nullable=False,
    ),
    Column(
        "TagId",
        ForeignKey("Tags.Id", ondelete="CASCADE"),
        primary_key=True,
        nullable=False,
    ),
    Index("IX_BlogTag_TagId", "TagId"),
)
