

INSERT INTO "register_image" ( 
  "id"
  , "file_size"
  , "file_name"
  , "file_extension"
  , "file_path"
  , "md5_hash"
  , "ave_hash"
  , image_repository_id
  , "height_pix"
  , "width_pix"
  , "do_get"
  , "delete_date"
  , "created_at"
  , "updated_at"
) 
SELECT
  M_IMAGE_INFO.IMGINF_ID
  , M_IMAGE_INFO.FILE_SIZE
  , M_IMAGE_INFO.FILE_NAME
  , M_IMAGE_INFO.FILE_TYPE
  , M_IMAGE_INFO.FILE_PATH
  , M_IMAGE_INFO.HASH_VALUE
  , T_AVEHASH.AVEHASH
  , M_IMAGE_INFO.IMGREPO_ID
  , M_IMAGE_INFO.HEIGHT_PIX
  , M_IMAGE_INFO.WIDTH_PIX
  , M_IMAGE_INFO.DO_GET
  , CASE
      WHEN M_IMAGE_INFO.DELETE_FLG = 1 THEN M_IMAGE_INFO.DELETE_DATE
      ELSE NULL
    END
  , M_IMAGE_INFO.CREATE_LINE_DATE
  , M_IMAGE_INFO.UPDATE_LINE_DATE 
FROM M_IMAGE_INFO
INNER JOIN T_AVEHASH ON T_AVEHASH.IMGINF_ID = M_IMAGE_INFO.IMGINF_ID;

select * from register_image;

INSERT INTO "image_repository" ( 
  "id"
  , "repository_path"
  , "created_at"
  , "updated_at"
) 
SELECT
  M_IMAGE_REPOSITORY.IMGREPO_ID
  , M_IMAGE_REPOSITORY.REPO_PATH
  , M_IMAGE_REPOSITORY.CREATE_LINE_DATE
  , M_IMAGE_REPOSITORY.UPDATE_LINE_DATE 
FROM M_IMAGE_REPOSITORY;

select * from image_repository;

INSERT INTO "tag" ( 
  "id"
  , "name"
  , "ruby"
  , "search_count"
  , "assign_image_count"
  , "description"
  , "created_at"
  , "updated_at"
) 
SELECT
  M_TAG_INFO.TAGINF_ID
  , M_TAG_INFO.TAG_NAME
  , M_TAG_INFO.TAG_RUBY
  , M_TAG_INFO.SEARCH_COUNT
  , M_TAG_INFO.IMG_COUNT
  , M_TAG_INFO.TAG_DEFINE
  , M_TAG_INFO.CREATE_LINE_DATE
  , M_TAG_INFO.UPDATE_LINE_DATE 
FROM M_TAG_INFO;

select * from tag;

INSERT INTO "tag_assign" ( 
  "register_image_id"
  , "tag_id"
  , "created_at"
  , "updated_at"
) 
SELECT
  T_ADD_TAG.IMGINF_ID
  , T_ADD_TAG.TAGINF_ID
  , T_ADD_TAG.CREATE_LINE_DATE
  , T_ADD_TAG.UPDATE_LINE_DATE 
FROM T_ADD_TAG;

select * from tag_assign;


INSERT INTO "tag_group" ("id", "name", "created_at", "updated_at") 
SELECT
  M_TAGADDTAB.TABSET_ID
  , M_TAGADDTAB.TAB_NAME
  , M_TAGADDTAB.CREATE_LINE_DATE
  , M_TAGADDTAB.UPDATE_LINE_DATE 
FROM
  M_TAGADDTAB;

select * from tag_group;

INSERT INTO "tag_group_assign" ( 
  "tag_group_id"
  , "tag_id"
  , "created_at"
  , "updated_at"
) 
SELECT
  M_TABSETTING.TABSET_ID
  , M_TABSETTING.TAGINF_ID
  , M_TABSETTING.CREATE_LINE_DATE
  , M_TABSETTING.UPDATE_LINE_DATE 
FROM M_TABSETTING; 

select * from tag_group_assign;


INSERT INTO "view_history" ( 
  "id"
  , "register_image_id"
  , "view_timestamp"
  , "view_time_ms"
  , "created_at"
  , "updated_at"
) 
SELECT
  T_VIEW_HISTORY.VIEWH_ID
  , T_VIEW_HISTORY.IMGINF_ID
  , T_VIEW_HISTORY.VIEW_DATE
  , T_VIEW_HISTORY.VIEW_TIME_MS
  , T_VIEW_HISTORY.CREATE_LINE_DATE
  , T_VIEW_HISTORY.UPDATE_LINE_DATE 
FROM T_VIEW_HISTORY;

select * from view_history;
