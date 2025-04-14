import React, { useState } from 'react';
import { Upload, Button, Image, message, Card, Layout, Typography, Col, Row, Switch } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import axios from 'axios';

const { Title, Text, Paragraph } = Typography;
const { Header, Content } = Layout;
const API_URL = process.env.REACT_APP_API_PATH;

const getBase64 = (file) =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = (error) => reject(error);
  });

const App = () => {
  const [previewOpen, setPreviewOpen] = useState(false);
  const [fileList, setFileList] = useState([]);
  const [preview, setPreview] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [solve, setSolve] = useState(false);
  const [result, setResult] = useState(null);
  const [resultText, setResultText] = useState("");

  const handleSubmit = async () => {
    const formData = new FormData();
    formData.append('file', fileList[0].originFileObj);
    formData.append('solve', solve);

    setUploading(true);

    try {
      const res = await axios.post(`${API_URL}/upload`, formData);
      const data = res.data;

      setResult(data);

      if (solve) {
        if (data.result !== undefined) {
          // Expression is valid and solved
          setResultText(`${data.extractedText} = ${data.result}`);
        }
        else if (data.errors) {
          // Invalid expression - show errors
          const errorString = data.errors.join(", ");
          setResultText(`${data.extractedText}\nThis is not a valid math expression.\nErrors:\n${errorString}`);
        }
        else {
          setResultText(data.extractedText || "No text extracted.");
        }
      }
      else {
        // Just show the extracted text
        setResultText(data.extractedText || "No text extracted.");
      }

      message.success('Upload successful!');
    }
    catch (err) {
      message.error('Upload failed');
    }
    finally {
      setUploading(false);
    }
  };

  const handlePreview = async (file) => {
    if (!file.preview) {
      file.preview = await getBase64(file.originFileObj);
    }

    setPreview(file.preview);
    setPreviewOpen(true);
  };

  const handleChange = ({ fileList: newFileList }) =>
    setFileList(newFileList);

  return (
    <Layout>
      <Header style={{ backgroundColor: "#142361", alignContent: "center" }}>
        <Title level={3} style={{ color: "#ffffff", margin: 0 }}>
          Photo-To-Text App
        </Title>
      </Header>
      <Content
        style={{
          display: "flex",
          justifyContent: "center",
          alignContent: "center",
          padding: "50px 0",
          minHeight: "100vh",
          backgroundColor: "#f2f2f2"
        }}
      >
        <Card
          style={{
            padding: "0 50px",
            width: "60%",
          }}
        >
          <Title level={4}>
            Select image to start:
          </Title>
          <Row gutter={16} justify="center">
            <Upload
              beforeUpload={() => false}
              loading={uploading}
              listType="picture-card"
              fileList={fileList}
              accept="image/*"
              onChange={handleChange}
              onPreview={handlePreview}
              maxCount={1}
            >
              {fileList.length === 1 ? null :
                <Col>
                  <PlusOutlined />
                  <div style={{ marginTop: 8 }}>Upload</div>
                </Col>
              }
            </Upload>
            {preview && (
              <Image
                src={preview}
                wrapperStyle={{ display: 'none' }}
                preview={{
                  visible: previewOpen,
                  onVisibleChange: (visible) => setPreviewOpen(visible),
                  afterOpenChange: (visible) => !visible && setPreview(''),
                }}
              />
            )}
          </Row>

          <Row gutter={16} justify="center" style={{ marginTop: 50 }}>
            <Col
              span={12}
              style={{
                display: "flex",
                flexDirection: "row",
                justifyContent: "center"
              }}
            >
              <Title
                level={5}
                style={{ margin: "0 10px 0 0" }}
              >
                Solve Math Problem?
              </Title>
              <Switch
                disabled={fileList.length !== 1}
                checkedChildren={"Yes"}
                unCheckedChildren={"No"}
                onChange={(checked) => setSolve(checked)}
              />
            </Col>
            <Col span={12}>
              <Button
                style={{ width: "100%" }}
                type={"primary"}
                disabled={fileList.length !== 1}
                onClick={() => handleSubmit()}
              >
                Submit
              </Button>
            </Col>
          </Row>

          <Row gutter={16} style={{ marginTop: 50 }}>
            <Col span={24}>
              <Title level={4}>Result:</Title>
              <Card style={{ backgroundColor: "#e3e3ea", minHeight: 100, width: "100%" }}>

                {/* Extracted Text Section */}
                <Title level={5} style={{ marginTop: 0 }}>Extracted Text</Title>
                {result && result.extractedText ? (
                  <Paragraph copyable style={{ whiteSpace: 'pre-line' }}>
                    {result.extractedText}
                  </Paragraph>
                ) : (
                  <Text type="secondary">No Data</Text>
                )}

                {/* Math Solver Section */}
                <Title level={5}>Math Solver</Title>
                {solve && resultText ? (
                  <Paragraph style={{ whiteSpace: 'pre-line' }}>
                    {resultText}
                  </Paragraph>
                ) : (
                  <Text type="secondary">No Data</Text>
                )}
              </Card>
            </Col>
          </Row>

          <Row gutter={16} style={{ margin: "50px 0 50px 0" }}>
            <Col>
              <Title level={4}>How to Use This App:</Title>
              <Paragraph>
                <Text strong style={{ marginLeft: 30 }}>1. Upload a Photo</Text>
                <br />
                <Text>
                  Click the <Text code>Upload</Text> area and choose <Text strong>one image</Text> that contains the text you want to extract.
                </Text>
                <br /><br />

                <Text strong style={{ marginLeft: 30 }}>2. Choose the Mode</Text>
                <br />
                <Text>
                  After uploading, select whether the image contains a <Text italic>math problem</Text> that you want solved, or if you just want to extract the <Text italic>plain text</Text>.
                </Text>

                {/* Subpoints under 2 */}
                <Paragraph style={{ marginLeft: 50, marginTop: 10 }}>
                  <ul style={{ paddingLeft: 20 }}>
                    <li>
                      <Text>Math expression can include: <Text code>digits, +, -, *, /, (, ), [, ]</Text></Text>
                    </li>
                    <li>
                      <Text>Fraction numbers are <Text strong>not allowed</Text></Text>
                    </li>
                    <li>
                      <Text>Please <Text strong>do not put an "="</Text> sign at the end of your expression, and make sure it doesn’t include any extra text.</Text>
                    </li>
                  </ul>
                </Paragraph>

                <Text strong style={{ marginLeft: 30 }}>3. Submit</Text>
                <br />
                <Text>
                  Click the <Text code>Submit</Text> button to send the image. The app will return either the extracted text or the solved result, depending on your selection.
                </Text>
                <br /><br />

                <Text strong style={{ marginLeft: 30 }}>4. Wait for result</Text>
                <br />
                <Text>
                  The extracted text or the solved result will be shown under the <Text italic>upload section</Text>.
                </Text>
              </Paragraph>
            </Col>
          </Row>
        </Card>
      </Content>
    </Layout>
  );
};

export default App;
