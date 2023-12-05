import { UploadOutlined } from "@ant-design/icons";
import type { UploadFile, UploadProps } from "antd";
import { Button, message, Upload } from "antd";
import { useEffect, useState } from "react";

import styles from "./styles.module.scss";
import { RcFile } from "antd/es/upload";

interface DownloadedFileProps {
  name: string;
  url: string;
}

const UploadComponent = () => {
  const [fileList, setFileList] = useState<UploadFile<RcFile>[]>([]);
  const [downloadedFileList, setDownloadedFileList] = useState<
    DownloadedFileProps[]
  >([]);

  const checkFileType = (file: UploadFile, typeCompare: string) => {
    if (file.type) {
      return typeCompare.includes(file.type);
    }
  };

  const download = (file: string | Blob | RcFile) => {
    const data = new FormData();
    data.append("file", file as Blob);
    fetch("https://localhost:44382/File/ConvertToPdf", {
      method: "POST",
      body: data,
    })
      .then((response) => response.blob())
      .then((blob) => {
        const url = window.URL.createObjectURL(new Blob([blob]));

        setDownloadedFileList((state) => [
          ...state,
          { name: `${(file as RcFile).name}.pdf`, url: url },
        ]);

      })
      .catch(() => message.error("Не удалось обработать запрос"));
  };

  const props: UploadProps = {
    name: "file",
    customRequest(options) {
      download(options.file);
    },
    fileList: fileList,
    headers: {
      authorization: "authorization-text",
    },
    beforeUpload(file) {
      if (checkFileType(file, "text/html")) {
        setFileList((state) => [...state, file]);
      } else {
        message.error(`${file.name} is not HTML file`);
      }
    },
    onRemove(file) {
      setFileList([...fileList.filter((f) => f !== file)]);
    },
    onChange(info) {
      if (info.file.status !== "uploading") {
        console.log(info.file, info.fileList);
      }
      if (info.file.status === "done") {
        message.success(`${info.file.name} file uploaded successfully`);
        console.log(info.file.response);
      } else if (info.file.status === "error") {
        message.error(`${info.file.name} file upload failed.`);
      }
    },
  };

  return (
    <div className={styles.uploader}>
      <Upload {...props}>
        <Button icon={<UploadOutlined />}>Click to Upload</Button>
      </Upload>
      <div className={styles.downloaded}>
        {downloadedFileList.map((f, index) => (
          <a
            key={f.name + index}
            target="_blank"
            href={f.url}
            download={f.name}
          >
            {f.name}
          </a>
        ))}
      </div>
    </div>
  );
};

export default UploadComponent;
