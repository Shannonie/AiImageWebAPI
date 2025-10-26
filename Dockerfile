# Stability AI (GPU)
# Base Python image
FROM nvidia/cuda:12.1.105-cudnn8-runtime-ubuntu22.04

# Install system dependencies
RUN apt-get update && apt-get install -y \
    git \
    python3.10 \
    python3-pip \
    python3-venv \
    ffmpeg \
    libsm6 \
    libxext6 \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Install Python packages
# Upgrade pip
RUN python3 -m pip install --upgrade pip

# Install dependencies
RUN pip install \
    torch==2.4.1+cu121 torchvision==0.19.1+cu121 torchaudio==2.4.1+cu121 --index-url https://download.pytorch.org/whl/cu121 && \
    "numpy<2" \
    stability-sdk \
    python-multipart \
    pytorch-lightning==2.0.1 \
    kornia==0.6.9 \
    invisible-watermark \
    "scipy>=1.10.1" \
    xformers==0.0.27 \
    diffusers==0.30.2 \
    transformers==4.41.2 accelerate safetensors \
    fastapi uvicorn pillow matplotlib einops omegaconf transformers==4.19.1

# Clone Stability AI repo
RUN git clone https://github.com/Stability-AI/generative-models.git
WORKDIR /app/generative-models
RUN pip install -e .

# Copy FastAPI server code
WORKDIR /app
COPY . .

# Expose FastAPI port & start server
EXPOSE 8000
CMD ["uvicorn", "stabilityAI_server:app", "--host", "0.0.0.0", "--port", "8000"]

# Real-ESRGAN (GPU)
# Base image with CUDA 12.1
FROM nvidia/cuda:12.1.105-cudnn8-runtime-ubuntu22.04

# Install system dependencies
RUN apt-get update && apt-get install -y \
    git \
    python3.11 \
    python3-pip \
    ffmpeg \
    libsm6 \
    libxext6 \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Upgrade pip
RUN python3 -m pip install --upgrade pip

# Install Python dependencies
RUN pip install \
    torch==2.4.1+cu121 torchvision==0.19.1+cu121 --index-url https://download.pytorch.org/whl/cu121 && \
    basicsr facexlib gfpgan \
    fastapi uvicorn opencv-python \
    numpy==1.26.4

# Clone and install Real-ESRGAN
RUN git clone https://github.com/xinntao/Real-ESRGAN.git
WORKDIR /app/Real-ESRGAN
RUN python setup.py develop

# Fix for rgb_to_grayscale (optional, can also patch in code)
# Example: uncomment if needed in your setup
# RUN sed -i 's/from torchvision.transforms.functional import rgb_to_grayscale/from torchvision.transforms.functional_tensor import rgb_to_grayscale/' basicsr/data/degradations.py

# Copy FastAPI server code
WORKDIR /app
COPY . .

# Expose FastAPI port
EXPOSE 8001

# Command to run FastAPI server
CMD ["uvicorn", "realesrgan_server:app", "--host", "0.0.0.0", "--port", "8001"]

