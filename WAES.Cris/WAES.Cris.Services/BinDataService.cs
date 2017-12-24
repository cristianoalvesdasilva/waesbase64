using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WAES.Cris.DataAccess;
using WAES.Cris.Model;
using WAES.Cris.Model.DTO;
using WAES.Cris.Services.Extensions;

namespace WAES.Cris.Services
{
  /// <summary>
  /// Responsible for persisting and comparing <see cref="BinData"/> records.
  /// </summary>
  public class BinDataService : DataService, IBinDataService
  {
    /// <summary>
    /// Initializes a <see cref="BinDataService"/> instance with an injected <see cref="IDataEntitiesFactory"/> instance.
    /// </summary>
    /// <param name="dataEntitiesFactory"></param>
    public BinDataService(IDataEntitiesFactory dataEntitiesFactory) : base(dataEntitiesFactory)
    {
    }

    /// <summary>
    /// Asynchronously compares <see cref="Model.BinData.LeftContent"/> against <see cref="Model.BinData.RightContent"/>
    /// for an existing <see cref="Model.BinData"/> record based off of a given <paramref name="id"/>.
    /// </summary>
    /// <param name="id"><see cref="Model.BinData"/> unique identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a single instance of <see cref="DataDiffResultDto"/></returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown should
    /// <para>***No record be found for the given <paramref name="id"/></para>
    /// <para>***Either <see cref="BinData.LeftContent"/> or <see cref="BinData.LeftContent"/> be null/empty.</para>
    /// </exception>
    public async Task<DataDiffResultDto> CompareLeftAndRightAsync(long id)
    {
      var data = await this.Query<BinData>().SingleOrDefaultAsync(_ => _.Id == id);
      if (data == null)
      {
        throw new InvalidOperationException($"No record was found for id {id}.");
      }

      this.AssertDataForComparison(data);

      byte[] leftContent = Convert.FromBase64String(data.LeftContent);
      byte[] rightContent = Convert.FromBase64String(data.RightContent);

      var diffResult = new DataDiffResultDto();
      if (leftContent.SequenceEqual(rightContent))
      {
        diffResult.Message = "Left and Right are the same.";
      }
      else if (leftContent.Length != rightContent.Length)
      {
        diffResult.Message = "Left and Right have different sizes.";
      }
      else
      {
        diffResult.Message = "Left and Right have same size, but with different content.";
        diffResult.Length = leftContent.Length;
        diffResult.DiffOffsets = new List<int>();

        for (int i = 0; i < leftContent.Length; i++)
        {
          if (leftContent[i] != rightContent[i])
          {
            diffResult.DiffOffsets.Add(i);
          }
        }
      }

      return diffResult;
    }

    /// <summary>
    /// Asynchronously creates OR updates a record for the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data"><see cref="BinData"/> instance containing a valid <see cref="BinData.Id"/>,
    /// and also either <see cref="BinData.LeftContent"/> or <see cref="BinData.RightContent"/> populated.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpsertAsync(BinData data)
    {
      this.AssertDataForUpsert(data);

      using (this.NewDbContextScope())
      {
        var existingRecord = await this.DbContext.BinDataSet.SingleOrDefaultAsync(_ => _.Id == data.Id);
        if (existingRecord == null)
        {
          existingRecord = this.DbContext.BinDataSet.Add(data);
        }
        else
        {
          if (!data.LeftContent.IsBlank())
          {
            existingRecord.LeftContent = data.LeftContent;
          }

          if (!data.RightContent.IsBlank())
          {
            existingRecord.RightContent = data.RightContent;
          }
        }

        await this.DbContext.SaveChangesAsync();
      }
    }

    /// <summary>
    /// Asserts a <see cref="BinData"/> instance and throws and exception should it be invalid.
    /// </summary>
    /// <param name="data">Instance to be validated.</param>
    /// <exception cref="ArgumentNullException">Thrown should <paramref name="data"/> be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown should <see cref="BinData.Id"/> be lower than 1.</exception>
    /// <exception cref="ArgumentException">
    /// <para>Thrown should both <see cref="BinData.LeftContent"/> and <see cref="BinData.RightContent"/> be null/empty/white space.</para>
    /// <para>Thrown should either <see cref="BinData.LeftContent"/> or <see cref="BinData.RightContent"/> be an invalid base64 string.</para>
    /// </exception>
    private void AssertDataForUpsert(BinData data)
    {
      if (data == null)
      {
        throw new ArgumentNullException(nameof(data));
      }

      if (data.Id <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(data), "'Id' must be greater than zero.");
      }

      if (data.LeftContent.IsBlank() && data.RightContent.IsBlank())
      {
        throw new ArgumentException("Either 'left' or 'right' properties must be provided.", nameof(data));
      }

      if (!data.LeftContent.IsBlank())
      {
        this.AssertBase64String(data.LeftContent);
      }

      if (!data.RightContent.IsBlank())
      {
        this.AssertBase64String(data.RightContent);
      }
    }

    /// <summary>
    /// Asserts the given <paramref name="content"/> and throws an exception should it not be a valid base64 string.
    /// </summary>
    /// <param name="content">Content to be validated.</param>
    /// <exception cref="ArgumentException">Thrown should the content not be a valid base64 string.</exception>
    private void AssertBase64String(string content)
    {
      try
      {
        Convert.FromBase64String(content);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException("Invalid base64 string.", ex);
      }
    }

    /// <summary>
    /// Asserts <see cref="BinData.LeftContent"/> and <see cref="BinData.RightContent"/>, and throws an exception should either of them be invalid.
    /// </summary>
    /// <param name="data"><see cref="BinData"/> to be validated.</param>
    /// <exception cref="InvalidOperationException">Thrown should either <see cref="BinData.LeftContent"/> or <see cref="BinData.LeftContent"/> be null/empty.</exception>
    private void AssertDataForComparison(BinData data)
    {
      if (data.LeftContent.IsBlank())
      {
        throw new InvalidOperationException("'Left' content is missing.");
      }

      if (data.RightContent.IsBlank())
      {
        throw new InvalidOperationException("'Right' content is missing.");
      }
    }
  }
}